using System;
using System.Text;
using System.Net.Sockets;
using SDCSCommon;
using System.Threading;

namespace Client
{
	/// <summary>
	/// An instance of this class is passed when a DataReceived event is raised.
	/// </summary>
	public class DataReceivedEventArgs : EventArgs
	{
		/// <summary>
		/// The header received from the server
		/// </summary>
		public Network.Header Header;
		/// <summary>
		/// The data received from the server
		/// </summary>
		public byte[] Data;
	}

	/// <summary>
	/// Contains functions for the client program to communicate over the network
	/// </summary>
	public class ClientNetwork
	{
		/// <summary>
		/// Create a delegate of this type and add it to the DataReceived list to be notified on data received events
		/// </summary>
		public delegate void DataReceivedDelegate(object sender, DataReceivedEventArgs e);

		/// <summary>
		/// The number of milliseconds to wait when reading data before declaring it a timeout
		/// </summary>
		const int MAX_READ_WAIT_TIME = 10000;
		/// <summary>
		/// The number of milliseconds between sending keep alive signals
		/// </summary>
		const int KEEP_ALIVE_TIME = 10000;

		/// <summary>
		/// Keeps track of the last time activity occured on this network connection
		/// </summary>
		private static System.DateTime lastActivity = System.DateTime.Now;

		/// <summary>
		/// This event is raised when new data is received from the server. <seealso cref="Client.ClientNetwork.DataReceivedDelegate"/> <seealso cref="Client.DataReceivedEventArgs"/>
		/// </summary>
		/// <example><code>
		///	public int main()
		///	{
		///		ClientNetwork.DataReceived += new ClientNetwork.DataReceivedDelegate(DataReceivedHandler);
		///	}
		///	
		///	public void DataReceivedHandler(object o, DataReceivedEventArgs e)
		///	{
		///		// Add code (most likely a switch statement) here that handles the different data that could be received
		///		// Data in e will always be decoded before being passed to the handler functions
		///		...
		///	}</code></example>
		public static event DataReceivedDelegate DataReceived;

		/// <summary>
		/// If null it means that a connection has not been made yet. Otherwise this is the stream through which communication to the server is performed
		/// </summary>
		private static NetworkStream connectionStream = null;

		/// <summary>
		/// The IPAddress or host name of the server. The default will connect to the main host though this should be different for each company/organization
		/// </summary>
		public static string IPAddress = "sdcscvs.getmyip.com";

		/// <summary>
		/// The remote host's port to connect to. By default this is set to 3000 and shouldn't be changed unless conflicting with other software
		/// </summary>
		public static int Port = 3000;

		/// <summary>
		/// The currently logged in user's username. Blank if no user is logged in. <seealso cref="Client.ClientNetwork.Username"/>
		/// </summary>
		private static string username = "";
		/// <summary>
		/// Gets the currently logged in user's username. Defaults to "" if no user is logged in.
		/// </summary>
		public static string Username
		{
			get
			{
				return username;
			}
		}

		/// <summary>
		/// Thread that listens for incoming data from the server
		/// </summary>
		private static Thread listeningThread;

		/// <summary>
		/// <see cref="Connected"/>
		/// </summary>
		private static bool connected = false;
		/// <summary>
		/// If true then the client is currently connected to the server, false otherwise.
		/// </summary>
		public static bool Connected
		{
			get
			{
				return connected;
			}
			set
			{
				connected = value;
			}
		}

		/// <summary>
		/// Connects to the server and then logs you in
		/// </summary>
		/// <param name="un">The user's username</param>
		/// <param name="password">The user's password</param>
		/// <returns>True if the login is successful, false otherwise</returns>
		/// <example><code>if (!ClientNetwork.logInToServer("Username", "Password"))
		///		MessageBox.Show("Log in failed");</code></example>
		public static bool logInToServer(string un, string password)
		{
			// Close any existing connections
			Disconnect(false);

			Network.Header head;
			string randomCodeData = "";

			try // Connect to the server
			{
				TcpClient client = new TcpClient(IPAddress, Port);
				connectionStream = client.GetStream();
				connected = true;

				byte[] header = new byte[Network.HEADER_SIZE];
				for (int i = 0; i < Network.HEADER_SIZE; i++)
				{
					while (connectionStream.DataAvailable == false)
					{} // ToDo: Add escape code
					header[i] = (byte)connectionStream.ReadByte();
				}

				head = Network.bytesToHeader(header);

				byte[] randomCode = new byte[head.Length]; // Download the random pass code for security
				for (int i = 0; i < head.Length; i++)
				{
					while (connectionStream.DataAvailable == false)
					{} // ToDo: Add escape code
					randomCode[i] = (byte)connectionStream.ReadByte();
				}

				randomCodeData = System.Text.UnicodeEncoding.Unicode.GetString(randomCode, 0, (int)head.Length);
			}
			catch
			{
				//System.Windows.Forms.MessageBox.Show("Could not connect to host");
				connected = false;
				return false;
			}

			if (head.DataType != Network.DataTypes.RandomPassCode) // Wrong type for first packet
			{
				//System.Windows.Forms.MessageBox.Show("Illegal data received from host");
				connected = false;
				return false;
			}

			// Create secure password
			password = SDCSCommon.CryptoFunctions.getMD5Hash(String.Concat(SDCSCommon.CryptoFunctions.getMD5Hash(password), randomCodeData));
			Network.Header sendHead = new Network.Header();
			sendHead.DataType = Network.DataTypes.LoginInformation;
			sendHead.FromID = 0;
			sendHead.ToID = -1;

			byte[] passBytes = System.Text.UnicodeEncoding.Unicode.GetBytes(password);
			byte[] usernameBytes = System.Text.UnicodeEncoding.Unicode.GetBytes(un);
			sendHead.Length = passBytes.Length + usernameBytes.Length + 4;

			SendData(SDCSCommon.Network.headerToBytes(sendHead), BitConverter.GetBytes(usernameBytes.Length), usernameBytes, passBytes);

			byte[] statusHeader = new byte[Network.HEADER_SIZE];
			for (int i = 0; i < Network.HEADER_SIZE; i++)
			{
				while (connectionStream.DataAvailable == false)
				{} // ToDo: Add escape code
				statusHeader[i] = (byte)connectionStream.ReadByte();
			}

			Network.Header statusHead = Network.bytesToHeader(statusHeader);

			byte[] statusCode = new byte[statusHead.Length]; // Download the random pass code for security
			for (int i = 0; i < statusHead.Length; i++)
			{
				while (connectionStream.DataAvailable == false)
				{} // ToDo: Add escape code
				statusCode[i] = (byte)connectionStream.ReadByte();
			}

			if (!(statusHead.DataType == Network.DataTypes.LoginStatus && BitConverter.ToInt32(statusCode, 0) == Network.LoginOK))
			{
				connected = false;
				try
				{
					connectionStream.Close();
				}
				catch
				{}
				return false;
			}

			username = un;
			
			listeningThread = new Thread(new ThreadStart(listeningFunc));
			listeningThread.Start();
			return true;
		}

		/// <summary>
		/// Call to disconnect from the server.
		/// </summary>
		/// <remarks>Always call disconnect before exiting the program. Failure to do so could cause a zombie thread.
		/// Also note that this sets the <see cref="Client.ClientNetwork.Username">Username</see> to blank and makes <see cref="Client.ClientNetwork.Connected">Connected</see> false.</remarks>
		/// <example>Always call this function on exit.
		/// <code>protected override void Dispose( bool disposing )
		///	{
		///		// Always call this before exiting the program
		///		ClientNetwork.Disconnect();
		///	}</code></example>
		public static void Disconnect(bool forced)
		{
			try
			{
				// Try to close the stream
				connectionStream.Close();
			}
			catch
			{}
			username = "";
			connected = false;

			if (forced)
			{
				Network.Header logoffHeader = new SDCSCommon.Network.Header();
				logoffHeader.DataType = Network.DataTypes.Logout;
				logoffHeader.Encrypted = false;
				logoffHeader.Length = 0;

				DataReceivedEventArgs e = new DataReceivedEventArgs();
				e.Header = logoffHeader;
				e.Data = System.Text.UnicodeEncoding.Unicode.GetBytes("Lost connection to server");

				DataReceived(null, e);
			}

			try
			{
				// Try to kill the thread
				listeningThread.Abort();
			}
			catch
			{}
		}

		/// <summary>
		/// Sends an IM to the user specified by the toID with the message passed to this function.
		/// </summary>
		/// <param name="toID">The userID of the user this message is to be sent to</param>
		/// <param name="message">The message to send</param>
		/// <returns>True if the sending was successful (doesn't mean the user received it, only that it was sent to the server), false otherwise.</returns>
		/// <example><code>public void btnSendIM_clicked(object o, EventArgs e)
		/// {
		///		// This assumes that a function for looking up a user ID has been written and that txtUsername and txtMessage are text boxes.
		///		if (!SendIM(lookupUserID(txtUsername.Text), txtMessage.Text))
		///			MessageBox.Show("Connection to server lost");
		/// }</code></example>
		public static bool SendIM(int toID, string message)
		{
			Network.Header head = new Network.Header();
			// FromID is set at the server
			head.FromID = 0;
			head.ToID = toID;
			head.DataType = Network.DataTypes.InstantMessage;
			
			byte[] data = System.Text.UnicodeEncoding.Unicode.GetBytes(message);
			head.Length = data.Length;
			return SendData(Network.headerToBytes(head), data);
		}

		/// <summary>
		/// Function for sending data to the server. All data being sent to the server should go through this function
		/// </summary>
		/// <param name="data">Takes an arbitrary number of byte arrays as data</param>
		/// <returns>True if the sending is successful, false otherwise</returns>
		public static bool SendData(params byte[][] data)
		{
			try
			{
				lock (connectionStream)
					foreach (byte [] dat in data)
						connectionStream.Write(dat, 0, dat.Length);
			}
			catch
			{
				Disconnect(true);
				return false;
			}

			lastActivity = System.DateTime.Now;
			return true;
		}

		/// <summary>
		/// Function for the listening thread to live in
		/// </summary>
		private static void listeningFunc()
		{
			// Records the last time a byte was received so we can check for a read timeout
			System.DateTime lastRead;

			// Loop for the lifetime of the thread
			while (true)
			{
				while (connectionStream.DataAvailable != true)
				{
					if (connected == false)
						return;
					if (System.DateTime.Now - lastActivity > System.TimeSpan.FromMilliseconds(KEEP_ALIVE_TIME))
						sendKeepAlive();
					Thread.Sleep(100);
				}

				// Get the header
				byte[] headerBuffer = new byte[Network.HEADER_SIZE];
				lastRead = System.DateTime.Now;
				for (int i = 0; i < Network.HEADER_SIZE; i++)
				{
					while (connectionStream.DataAvailable != true)
					{
						if (connected == false)
							return;
						if (System.DateTime.Now - lastRead > System.TimeSpan.FromMilliseconds(MAX_READ_WAIT_TIME))
							Disconnect(true);
						Thread.Sleep(100);
					}
					headerBuffer[i] = (byte)connectionStream.ReadByte();
					lastRead = System.DateTime.Now;
				}

				// Get the data
				Network.Header head = Network.bytesToHeader(headerBuffer);
				byte[] data = new byte[head.Length];

				for (int i = 0; i < head.Length; i++)
				{
					while (connectionStream.DataAvailable != true)
					{
						if (connected == false)
							return;
						if (System.DateTime.Now - lastRead > System.TimeSpan.FromMilliseconds(MAX_READ_WAIT_TIME))
							Disconnect(true);
						Thread.Sleep(100);
					}
					data[i] = (byte)connectionStream.ReadByte();
					lastRead = System.DateTime.Now;
				}

				// Raise the DataReceivedEvent to let the gui know new data has arrived
				DataReceivedEventArgs eventArgs = new DataReceivedEventArgs();
				eventArgs.Header = head;
				eventArgs.Data = data;

				DataReceived(null, eventArgs);

				// Sleep to let other threads do their thing
				Thread.Sleep(100);
				lastActivity = System.DateTime.Now;
			}
		}

		/// <summary>
		/// Sends the keepAlive signal to check if the network connection is still active
		/// </summary>
		private static void sendKeepAlive()
		{
			Network.Header pingHead = new SDCSCommon.Network.Header();
			pingHead.DataType = Network.DataTypes.Ping;
			pingHead.Encrypted = false;
			pingHead.FromID = 0;
			pingHead.Length = 0;
			pingHead.ToID = -1;

			SendData(Network.headerToBytes(pingHead));
		}
	}
}

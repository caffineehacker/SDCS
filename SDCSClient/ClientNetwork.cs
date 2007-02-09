/* $Id$
 * $Log$
 * Revision 1.13  2007/02/09 18:08:47  tim
 * Updated documentation
 *
 * Revision 1.12  2007-02-09 17:39:00  tim
 * Updated documentation
 *
 * Revision 1.11  2007-02-06 21:33:30  tim
 * Tracked down a bug that was cripling the network communications and implemented most of the rest of the buddy list network code
 *
 * Revision 1.10  2007-02-05 05:08:07  tim
 * Updated comments and moved some code from GUI files to ClientNetwork.cs
 *
 * Revision 1.9  2007-02-04 05:28:53  tim
 * Updated all of the XML comments
 *
 * Revision 1.8  2007-02-04 04:21:45  tim
 * Added comments to better explain the code and fixed a spelling mistake in a function name
 *
 * Revision 1.7  2007-02-01 18:13:55  tim
 * Final touches on password authentication and fixing a server crash bug
 *
 * Revision 1.6  2007-02-01 17:56:43  tim
 * Reworked the login system to use usernames and passwords
 *
 * Revision 1.5  2007-02-01 17:18:43  tim
 * Changed the login process to use usernames and passwords
 *
 * Revision 1.4  2007-02-01 14:53:02  tim
 * Fixed a little bug where I used "=" instead of "=="
 *
 * Revision 1.3  2007-02-01 12:00:55  tim
 * Added CVS keywords
 *
 */

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
		public static string IPAddress = "localhost";

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
		/// <param name="username">The user's username</param>
		/// <param name="password">The user's password</param>
		/// <returns>True if the login is successful, false otherwise</returns>
		/// <example><code>if (!ClientNetwork.logInToServer("Username", "Password"))
		///		MessageBox.Show("Log in failed");</code></example>
		public static bool logInToServer(string username, string password)
		{
			// Close any existing connections
			Disconnect();

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
			byte[] usernameBytes = System.Text.UnicodeEncoding.Unicode.GetBytes(username);
			sendHead.Length = passBytes.Length + usernameBytes.Length + 4;

			connectionStream.Write(SDCSCommon.Network.headerToBytes(sendHead), 0, SDCSCommon.Network.HEADER_SIZE);
			connectionStream.Write(BitConverter.GetBytes(usernameBytes.Length), 0, 4);
			connectionStream.Write(usernameBytes, 0, usernameBytes.Length);
			connectionStream.Write(passBytes,0,passBytes.Length);

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
		public static void Disconnect()
		{
			try
			{
				// Try to kill the thread
				listeningThread.Abort();
			}
			catch
			{}
			try
			{
				// Try to close the stream
				connectionStream.Close();
			}
			catch
			{}
			username = "";
			connected = false;
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
			if (connected)
			{
				Network.Header head = new Network.Header();
				// FromID is set at the server
				head.FromID = 0;
				head.ToID = toID;
				head.DataType = Network.DataTypes.InstantMessage;
			
				byte[] data = System.Text.UnicodeEncoding.Unicode.GetBytes(message);
				head.Length = data.Length;
				connectionStream.Write(Network.headerToBytes(head), 0, Network.HEADER_SIZE);
				connectionStream.Write(data, 0, data.Length);

				return true;
			}
			
			// Disconnect from the server if the data can't be sent
			Disconnect();
			return false;
		}

		/// <summary>
		/// Function for the listening thread to live in
		/// </summary>
		private static void listeningFunc()
		{
			// Loop for the lifetime of the thread
			while (true)
			{
				while (connectionStream.DataAvailable != true)
				{Thread.Sleep(0);
					if (connected == false)
						return;
				}

				// Get the header
				byte[] headerBuffer = new byte[Network.HEADER_SIZE];
				for (int i = 0; i < Network.HEADER_SIZE; i++)
				{
					while (connectionStream.DataAvailable != true)
					{Thread.Sleep(0);
						if (connected == false)
							return;
					}
					headerBuffer[i] = (byte)connectionStream.ReadByte();
				}

				// Get the data
				Network.Header head = Network.bytesToHeader(headerBuffer);
				byte[] data = new byte[head.Length];

				for (int i = 0; i < head.Length; i++)
				{
					while (connectionStream.DataAvailable != true)
					{Thread.Sleep(0);
						if (connected == false)
							return;
					}
					data[i] = (byte)connectionStream.ReadByte();
				}

				// Raise the DataReceivedEvent to let the gui know new data has arrived
				DataReceivedEventArgs eventArgs = new DataReceivedEventArgs();
				eventArgs.Header = head;
				eventArgs.Data = data;

				DataReceived(null, eventArgs);

				// Sleep to let other threads do their thing
				Thread.Sleep(0);
			}
		}
	}
}

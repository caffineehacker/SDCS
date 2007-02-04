/* $Id$
 * $Log$
 * Revision 1.9  2007/02/04 05:28:53  tim
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

	class ClientNetwork
	{
		public delegate void DataReceivedDelegate(object sender, DataReceivedEventArgs e);

		public static event DataReceivedDelegate DataReceived;
		public static NetworkStream connectionStream = null;
		public static int bufferSize = 0;

		public static string IPAddress = "sdcscvs.getmyip.com";
		public static int Port = 3000;

		public static string Username = "";

		public static Thread listeningThread = new Thread(new ThreadStart(listeningFunc));

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
				bufferSize = client.ReceiveBufferSize;
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
			
			listeningThread.Start();
			return true;
		}

		/// <summary>
		/// Call to disconnect from the server
		/// </summary>
		public static void Disconnect()
		{
			try
			{
				listeningThread.Abort();
			}
			catch
			{}
			try
			{
				connectionStream.Close();
			}
			catch
			{}
			connected = false;
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

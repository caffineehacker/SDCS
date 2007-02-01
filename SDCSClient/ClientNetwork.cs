/* $Id$
 * $Log$
 * Revision 1.5  2007/02/01 17:18:43  tim
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
	public class DataReceivedEventArgs : EventArgs
	{
		public Network.Header Header;
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
		/// <param name="userName">The user's username</param>
		/// <param name="password">The user's password</param>
		/// <returns>True if the login is successful, false otherwise</returns>
		public static bool logInToServer(string userName, string password)
		{
			try
			{
				connectionStream.Close();
			}
			catch
			{
			}

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
				System.Windows.Forms.MessageBox.Show("Could not connect to host");
				connected = false;
				return false;
			}

			if (head.DataType != Network.DataTypes.RandomPassCode) // Wrong type for first packet
			{
				System.Windows.Forms.MessageBox.Show("Illegal data received from host");
				connected = false;
				return false;
			}

			// Create secure password
			password = SDCSCommon.CryptoFunctions.getMD5Hash(String.Concat(password, randomCodeData));
			Network.Header sendHead = new Network.Header();
			sendHead.DataType = Network.DataTypes.LoginInformation;
			sendHead.FromID = 0;
			sendHead.ToID = -1;
			sendHead.Length = 0;

			connectionStream.Write(SDCSCommon.Network.headerToBytes(sendHead), 0, SDCSCommon.Network.HEADER_SIZE);
			byte[] passBytes = System.Text.UnicodeEncoding.Unicode.GetBytes(password);
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

		public static void Disconect()
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

		private static void listeningFunc()
		{
			while (true)
			{
				while (connectionStream.DataAvailable != true)
				{Thread.Sleep(0);
					if (connected == false)
						return;
				}
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

				DataReceivedEventArgs eventArgs = new DataReceivedEventArgs();
				eventArgs.Header = head;
				eventArgs.Data = data;

				DataReceived(null, eventArgs);

				Thread.Sleep(0);
			}
		}
	}
}

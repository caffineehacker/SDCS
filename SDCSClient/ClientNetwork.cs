/* $Id$
 * $Log$
 * Revision 1.3  2007/02/01 12:00:55  tim
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

		/// <summary>
		/// UserID received from the server
		/// </summary>
		public static int userID;

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
		/// Connects to the server
		/// </summary>
		/// <param name="IPAddress">IP address of the server</param>
		/// <param name="port">Port number on the server to connect to</param>
		/// <returns>True if connection is successful, false otherwise</returns>
		public static bool connectToHost()
		{
			try
			{
				connectionStream.Close();
			}
			catch
			{
			}

			try
			{
				TcpClient client = new TcpClient(IPAddress, Port);
				connectionStream = client.GetStream();
				bufferSize = client.ReceiveBufferSize;
				connected = true;

				byte[] header = new byte[Network.HEADER_SIZE];
				for (int i = 0; i < Network.HEADER_SIZE; i++)
				{
					while (connectionStream.DataAvailable == false)
					{}
					header[i] = (byte)connectionStream.ReadByte();
				}

				Network.Header head = Network.bytesToHeader(header);
				userID = head.ToID;

				listeningThread.Start();
				return true;
			}
			catch
			{
				System.Windows.Forms.MessageBox.Show("Could not connect to host");
				connected = false;
				return false;
			}
		}

		/// <summary>
		/// Logs in to the server after you have connected using connectToServer(...)
		/// </summary>
		/// <param name="userName">The user's username</param>
		/// <param name="password">The user's password</param>
		/// <returns>True if the login is successful, false otherwise</returns>
		public static bool logInToServer(string userName, string password)
		{
			return false;
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
					if (connected = false)
						return;
				}
				byte[] headerBuffer = new byte[Network.HEADER_SIZE];
				for (int i = 0; i < Network.HEADER_SIZE; i++)
				{
					while (connectionStream.DataAvailable != true)
					{Thread.Sleep(0);
						if (connected = false)
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
						if (connected = false)
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

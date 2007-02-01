/* $Id$
 * $Log$
 * Revision 1.3  2007/02/01 17:18:43  tim
 * Changed the login process to use usernames and passwords
 *
 * Revision 1.2  2007-02-01 12:00:55  tim
 * Added CVS keywords
 *
 */

using System;
using System.Threading;
using SDCSCommon;

namespace Server
{
	/// <summary>
	/// This class is responsible for watching the connections for incoming data
	/// </summary>
	public class ConnectionWatcher
	{
		public Thread watchingThread;
		private ServerNetwork.connection conn;

		/// <summary>
		/// Default empty constructor
		/// </summary>
		public ConnectionWatcher()
		{
		}

		public void sendData(byte[] data)
		{
			try
			{
				conn.stream.Write(data, 0, data.Length);
			}
			catch // Close the connection
			{
			}
		}

		public ConnectionWatcher(ServerNetwork.connection con)
		{
			conn = con;

			watchingThread = new Thread(new ThreadStart(connectionWatcherFunc));
			watchingThread.Start();
		}

		/// <summary>
		/// Function for the connection watching thread to live in. Basically is a message pump for the network
		/// </summary>
		private void connectionWatcherFunc()
		{
			while (true)
			{
				while (conn.stream.DataAvailable == false)
				{if (frmServer.ShuttingDown)
					 return;
					Thread.Sleep(100);}
				byte[] headerBuffer = new byte[Network.HEADER_SIZE];
				for (int i = 0; i < Network.HEADER_SIZE; i++)
				{
					while (conn.stream.DataAvailable == false)
					{if (frmServer.ShuttingDown)
						 return;
						Thread.Sleep(100);}
					headerBuffer[i] = (byte)conn.stream.ReadByte();
				}

				Network.Header head = Network.bytesToHeader(headerBuffer);

				// This is to prevent a user from spoofing themselves as another user
				head.FromID = conn.userID;

				byte[] data = new byte[head.Length];

				for (int i = 0; i < head.Length; i++)
				{
					while (conn.stream.DataAvailable == false)
					{if (frmServer.ShuttingDown)
						 return;
						Thread.Sleep(100);}
					data[i] = (byte)conn.stream.ReadByte();
				}

				switch (head.DataType)
				{
					case Network.DataTypes.InstantMessage:
						for (int i = 0; i < ServerNetwork.netStreams.Count; i++)
						{
							if (((ServerNetwork.connection)ServerNetwork.netStreams[i]).userID == head.ToID)
							{
								System.Collections.ArrayList outgoing = new System.Collections.ArrayList();
								outgoing.AddRange(Network.headerToBytes(head));
								outgoing.AddRange(data);

								((ServerNetwork.connection)ServerNetwork.netStreams[i]).stream.Write((byte[])outgoing.ToArray(typeof(byte)), 0, outgoing.Count);
							}
						}
						break;
					case Network.DataTypes.WhiteBoard:
						break;
					default:
						break;
				}
			}
		}
	}
}

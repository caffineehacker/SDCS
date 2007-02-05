/* $Id$
 * $Log$
 * Revision 1.9  2007/02/05 19:33:54  tim
 * Some code cleanups for readability
 *
 * Revision 1.8  2007-02-04 05:28:53  tim
 * Updated all of the XML comments
 *
 * Revision 1.7  2007-02-04 04:30:55  tim
 * More shutdown code changes
 *
 * Revision 1.6  2007-02-04 04:21:45  tim
 * Added comments to better explain the code and fixed a spelling mistake in a function name
 *
 * Revision 1.5  2007-02-01 18:13:55  tim
 * Final touches on password authentication and fixing a server crash bug
 *
 * Revision 1.4  2007-02-01 17:56:43  tim
 * Reworked the login system to use usernames and passwords
 *
 * Revision 1.3  2007-02-01 17:18:43  tim
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
		private Thread watchingThread;
		private ServerNetwork.connection conn;

		/// <summary>
		/// Stores the random code so that login information can be confirmed
		/// </summary>
		private byte[] randomCode;

		/// <summary>
		/// Until true the client is not allowed to do anything
		/// </summary>
		private bool loggedIn = false;

		/// <summary>
		/// Default empty constructor
		/// </summary>
		public ConnectionWatcher()
		{
		}

		/// <summary>
		/// Call when shutting down the server
		/// </summary>
		public void Shutdown()
		{
			try
			{
				watchingThread.Abort();
			}
			catch
			{}
		}

		/// <summary>
		/// Send data to the client
		/// </summary>
		/// <param name="data">The array of bytes to be sent</param>
		private void sendData(byte[] data)
		{
			try
			{
				conn.stream.Write(data, 0, data.Length);
			}
			catch // ToDo: Close the connection
			{
				Shutdown();
			}
		}

		/// <summary>
		/// Prefered constructor to be used
		/// </summary>
		/// <param name="con">The connection that this instance is based on</param>
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
			// First send the random code for the client
			Network.Header sendHead = new SDCSCommon.Network.Header();
			sendHead.DataType = Network.DataTypes.RandomPassCode;
			sendHead.FromID = -1;
			sendHead.ToID = 0;
			sendHead.Length = 32;

			// Create a random 32 byte unicode string
			randomCode = new byte[32];
			Random rand = new Random();
			rand.NextBytes(randomCode);

			// And send it to the client
			sendData(Network.headerToBytes(sendHead));
			sendData(randomCode);

			// Now loop forever
			while (true)
			{
				// Each while loop continually checks if the server is shutting down.
				// This is to make sure that we don't end up with zombie threads.
				while (conn.stream.DataAvailable == false)
				{if (ServerNetwork.ShuttingDown)
					 return;
					Thread.Sleep(100);}
				byte[] headerBuffer = new byte[Network.HEADER_SIZE];
				for (int i = 0; i < headerBuffer.Length; i++)
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

				// Read in exactly the amount of data sent
				byte[] data = new byte[head.Length];
				for (int i = 0; i < head.Length; i++)
				{
					while (conn.stream.DataAvailable == false)
					{if (frmServer.ShuttingDown)
						 return;
						Thread.Sleep(100);}
					data[i] = (byte)conn.stream.ReadByte();
				}

				if (loggedIn || head.DataType == Network.DataTypes.LoginInformation)
				{
					switch (head.DataType)
					{
						case Network.DataTypes.InstantMessage: // Instant message data
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
						case Network.DataTypes.WhiteBoard: // Whiteboard drawing information
							break;
						case Network.DataTypes.LoginInformation: // Initial login
							// We receive a double hased password from the client. We store the single hash in the user database.
							// First we convert the received username and double hash in to unicode strings.
							int usernameLength = BitConverter.ToInt32(data,0);
							string username = System.Text.UnicodeEncoding.Unicode.GetString(data, 4, usernameLength);
							string password = System.Text.UnicodeEncoding.Unicode.GetString(data, 4 + usernameLength, data.Length - (4 + usernameLength));

							Network.Header confirmHead = new SDCSCommon.Network.Header();
							confirmHead.DataType = Network.DataTypes.LoginStatus;
							confirmHead.FromID = -1;
							confirmHead.Length = 4;

							// This statement performs the second hash on the stored password and compares it with the password received from
							// the client.
							if (CryptoFunctions.getMD5Hash(String.Concat(ServerDatabase.getUserPass(username), System.Text.UnicodeEncoding.Unicode.GetString(randomCode))) == password)
							{ // Login successful
								conn.userID = ServerDatabase.getUserID(username);
								confirmHead.ToID = conn.userID;

								// Send the Login OK message to let the client know they're authenticated
								sendData(Network.headerToBytes(confirmHead));
								sendData(BitConverter.GetBytes(Network.LoginOK));
								loggedIn = true;
							}
							else
							{ // Login failed
								confirmHead.ToID = 0;
								sendData(Network.headerToBytes(confirmHead));
								sendData(BitConverter.GetBytes(Network.LoginBad));
							}
							break;
						default:
							break;
					}
				}
			}
		}
	}
}

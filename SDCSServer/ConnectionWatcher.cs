using System;
using System.Threading;
using SDCSCommon;
using System.Collections;

namespace Server
{
	/// <summary>
	/// This class is responsible for watching the connections for incoming data
	/// </summary>
	public class ConnectionWatcher
	{
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
		private System.DateTime lastActivity = System.DateTime.Now;

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
		/// Add new buddy list data here. Only add objects of the type BuddyListData and only add data after setting AddingBuddyListData to true.
		/// </summary>
		public ArrayList BuddyListData = new ArrayList();

		/// <summary>
		/// Default empty constructor
		/// </summary>
		public ConnectionWatcher()
		{
		}

		/// <summary>
		/// Call when shutting down the server or to disconnect the client
		/// </summary>
		public void Shutdown()
		{
			loggedIn = false;
			try
			{
				conn.stream.Close();
			}
			catch
			{
			}

			ServerNetwork.netStreams.Remove(conn);
			ServerNetwork.notifyBuddyStatus(conn.userID, conn.username, Network.UserState.Offline);

			conn.userID = 0;
			conn.username = "";

			try
			{
				watchingThread.Abort();
			}
			catch
			{}
		}

		/// <summary>
		/// Sends data to the client
		/// </summary>
		/// <param name="data">The array of bytes to be sent, can take an arbitrary number of arrays</param>
		private void sendData(params byte [][] data)
		{
			try
			{
				lock (conn.stream)
					foreach (byte [] dat in data)
						conn.stream.Write(dat, 0, dat.Length);
			}
			catch // ToDo: Close the connection
			{
				Shutdown();
			}

			lastActivity = System.DateTime.Now;
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

		private void sendBuddyListData()
		{
			// This code effectively creates a semaphore
			lock (BuddyListData.SyncRoot)
			{
				Network.Header buddyHeader = new Network.Header();
				buddyHeader.DataType = Network.DataTypes.BuddyListUpdate;
				buddyHeader.FromID = -1;
				buddyHeader.ToID = conn.userID;

				byte[] buddyBytes = Network.BuddyListDataToBytes((Network.BuddyListData[])BuddyListData.ToArray(typeof(Network.BuddyListData)));

				buddyHeader.Length = buddyBytes.Length;
				sendData(Network.headerToBytes(buddyHeader), buddyBytes);

				BuddyListData.Clear();
			}
		}

		/// <summary>
		/// Function for the connection watching thread to live in. Basically is a message pump for the network
		/// </summary>
		private void connectionWatcherFunc()
		{
			// Records the last time a byte was received so we can check for a read timeout
			System.DateTime lastRead;

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
			sendData(Network.headerToBytes(sendHead), randomCode);

			// Now loop forever
			while (true)
			{
				// Each while loop continually checks if the server is shutting down.
				// This is to make sure that we don't end up with zombie threads.
				while (conn.stream.DataAvailable == false)
				{
					if (ServerNetwork.ShuttingDown)
                        return;
					if (BuddyListData.Count != 0)
						sendBuddyListData();
					if (System.DateTime.Now - lastActivity > TimeSpan.FromMilliseconds(KEEP_ALIVE_TIME))
						sendKeepAlive();
					Thread.Sleep(100);
				}

				byte[] headerBuffer = new byte[Network.HEADER_SIZE];
				lastRead = System.DateTime.Now;
				for (int i = 0; i < headerBuffer.Length; i++)
				{
					while (conn.stream.DataAvailable == false)
					{
						if (frmServer.ShuttingDown || (System.DateTime.Now - lastRead) > System.TimeSpan.FromMilliseconds(MAX_READ_WAIT_TIME))
							return;
						Thread.Sleep(100);
					}
					headerBuffer[i] = (byte)conn.stream.ReadByte();
					lastRead = System.DateTime.Now;
				}

				Network.Header head = Network.bytesToHeader(headerBuffer);

				// This is to prevent a user from spoofing themselves as another user
				head.FromID = conn.userID;

				// Read in exactly the amount of data sent
				byte[] data = new byte[head.Length];
				lastRead = System.DateTime.Now;
				for (int i = 0; i < head.Length; i++)
				{
					while (conn.stream.DataAvailable == false)
					{if (frmServer.ShuttingDown || (System.DateTime.Now - lastRead) > System.TimeSpan.FromMilliseconds(MAX_READ_WAIT_TIME))
						 return;
						Thread.Sleep(100);}
					data[i] = (byte)conn.stream.ReadByte();
					lastRead = System.DateTime.Now;
				}

				// We don't want to allow someone to do anything but log in until they are logged in
				if (loggedIn || head.DataType == Network.DataTypes.LoginInformation)
				{
					switch (head.DataType)
					{
						case Network.DataTypes.InstantMessage: // Instant message data
							for (int i = 0; i < ServerNetwork.netStreams.Count; i++)
							{
								if (((ServerNetwork.connection)ServerNetwork.netStreams[i]).userID == head.ToID)
								{
									((ServerNetwork.connection)ServerNetwork.netStreams[i]).watchingClass.sendData(Network.headerToBytes(head), data);
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
								conn.username = username;
								confirmHead.ToID = conn.userID;

								// Send the Login OK message to let the client know they're authenticated
								sendData(Network.headerToBytes(confirmHead), BitConverter.GetBytes(Network.LoginOK));
								loggedIn = true;

								// Let everyone know that the user is now online
								conn.userState = SDCSCommon.Network.UserState.Online;
								ServerNetwork.notifyBuddyStatus(conn.userID, conn.username, Network.UserState.Online);
								ServerNetwork.refreshBuddyList(conn);
							}
							else
							{ // Login failed
								confirmHead.ToID = 0;
								sendData(Network.headerToBytes(confirmHead), BitConverter.GetBytes(Network.LoginBad));
								Shutdown();
							}
							break;
						default:
							break;
					}
				}

				lastActivity = System.DateTime.Now;
			}
		}

		/// <summary>
		/// Sends the keepAlive signal to check if the network connection is still active
		/// </summary>
		private void sendKeepAlive()
		{
			Network.Header pingHead = new SDCSCommon.Network.Header();
			pingHead.DataType = Network.DataTypes.Ping;
			pingHead.Encrypted = false;
			pingHead.FromID = -1;
			pingHead.Length = 0;
			pingHead.ToID = conn.userID;

			sendData(Network.headerToBytes(pingHead));
		}
	}
}

using System;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Net;

namespace Server
{
	/// <summary>
	/// This is the network code for the server
	/// </summary>
	public class ServerNetwork
	{
		/// <summary>
		/// Holds all the information about a connection including the communication stream, the class associated with this connection, and the user ID
		/// </summary>
		public class connection
		{
			/// <summary>
			/// The stream connected to the client
			/// </summary>
			public NetworkStream stream;
			/// <summary>
			/// The class responsible for this connection
			/// </summary>
			public ConnectionWatcher watchingClass;
			/// <summary>
			/// The user ID of the user logged in on this connection
			/// </summary>
			public int userID;
			/// <summary>
			/// The username of the user logged in on this connection
			/// </summary>
			public string username;
			/// <summary>
			/// The current state of the user
			/// </summary>
			public SDCSCommon.Network.UserState userState;
		}

		/// <summary>
		/// This arrayList holds data for all the active connections
		/// </summary>
		public static ArrayList netStreams = new ArrayList();
		/// <summary>
		/// The port we listen on for incoming connections
		/// </summary>
		private static int listeningPort = 3000;

		/// <summary>
		/// The thread responsible for watching for incoming connections
		/// </summary>
		public static Thread listeningThread = new Thread(new ThreadStart(listeningThreadFunc));

		/// <summary>
		/// Set to true when shutting down the server
		/// </summary>
		public static bool ShuttingDown = false;

		/// <summary>
		/// Constructor for ServerNetwork
		/// </summary>
		public ServerNetwork()
		{
		}

		/// <summary>
		/// Call when shutting down the server program to close the network and abort the threads
		/// </summary>
		public static void shutDown()
		{
			ShuttingDown = true;
			try
			{
				listeningThread.Abort();
			}
			catch
			{}

			lock (netStreams.SyncRoot)
				for (int i = 0; i < netStreams.Count; i++)
				{
					try
					{
						((connection)netStreams[i]).watchingClass.Shutdown();
					}
					catch
					{}
				}
		}

		/// <summary>
		/// Start listening for incoming connections
		/// </summary>
		public static void startListening()
		{
			try
			{
				listeningThread.Start();
			}
			catch
			{}
		}

		/// <summary>
		/// Start listening for incomming connection on the given port
		/// </summary>
		/// <param name="port">Port to listen on</param>
		public static void startListening(int port)
		{
			listeningPort = port;
			startListening();
		}

		/// <summary>
		/// Function for the listening thread to live in
		/// </summary>
		private static void listeningThreadFunc()
		{
			TcpListener listener = new TcpListener(IPAddress.Any, listeningPort);
			listener.Start();
			while(true)
			{
				while (listener.Pending() == false)
				{if (ShuttingDown)
					  return;
					 Thread.Sleep(100);}
				TcpClient client = listener.AcceptTcpClient();

				connection conn = new connection();
				conn.userID = 0;
				conn.stream = client.GetStream();
				lock (netStreams.SyncRoot)
					netStreams.Add(conn);
				conn.watchingClass = new ConnectionWatcher(conn);
			}
		}

		/// <summary>
		/// When a user state changes call this function to notify all the relevant clients
		/// </summary>
		/// <param name="userID">ID of the user who's state has changed</param>
		/// <param name="username">The username of the user who's state has changed</param>
		/// <param name="state">The new state of the user</param>
		public static void notifyBuddyStatus(int userID, string username, SDCSCommon.Network.UserState state)
		{
			SDCSCommon.Network.BuddyListData bld = new SDCSCommon.Network.BuddyListData();
			bld.userID = userID;
			bld.username = username;
			bld.userState = state;

			lock (netStreams.SyncRoot)
				for (int i = 0; i < netStreams.Count; i++)
				{
					lock (((connection)netStreams[i]).watchingClass.BuddyListData.SyncRoot)
					{
						((connection)netStreams[i]).watchingClass.BuddyListData.Add(bld);
					}
				}
		}

		/// <summary>
		/// Sends data on all users currently logged in
		/// </summary>
		/// <param name="con">The connection this data should be sent to</param>
		public static void refreshBuddyList(connection con)
		{
			lock (netStreams.SyncRoot)
				foreach (connection budCon in netStreams)
				{
					SDCSCommon.Network.BuddyListData bld = new SDCSCommon.Network.BuddyListData();
					bld.userID = budCon.userID;
					bld.username = budCon.username;
					bld.userState = budCon.userState;

					lock (con.watchingClass.BuddyListData.SyncRoot)
					{
						con.watchingClass.BuddyListData.Add(bld);
					}
				}
		}
	}
}

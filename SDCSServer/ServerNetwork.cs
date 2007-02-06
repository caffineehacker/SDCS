/* $Id$
 * $Log$
 * Revision 1.9  2007/02/06 16:28:15  tim
 * More code for the buddy list on the server side
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
 * Revision 1.5  2007-02-04 03:59:37  tim
 * Changed some shutdown code so that the UI and the core code are more seperated
 *
 * Revision 1.4  2007-02-01 17:56:43  tim
 * Reworked the login system to use usernames and passwords
 *
 * Revision 1.3  2007-02-01 16:19:41  tim
 * Added code for storing the user's data and adding a new user from the server program.
 *
 * Revision 1.2  2007-02-01 12:00:55  tim
 * Added CVS keywords
 *
 */

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
		public struct connection
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

			for (int i = 0; i < netStreams.Count; i++)
			{
				((connection)netStreams[i]).watchingClass.AddingBuddyListData = true;
				while (((connection)netStreams[i]).watchingClass.SendingBuddyListData)
				{}
				((connection)netStreams[i]).watchingClass.BuddyListData.Add(bld);
				((connection)netStreams[i]).watchingClass.BuddyListDataWaiting = true;
				((connection)netStreams[i]).watchingClass.AddingBuddyListData = false;
			}
		}
	}
}

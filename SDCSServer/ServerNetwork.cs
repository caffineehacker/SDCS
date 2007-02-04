/* $Id$
 * $Log$
 * Revision 1.6  2007/02/04 04:21:45  tim
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
		public struct connection
		{
			public NetworkStream stream;
			public ConnectionWatcher watchingClass;
			public int userID;
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
					((connection)netStreams[i]).watchingClass.watchingThread.Abort();
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
	}
}

/* $Id$
 * $Log$
 * Revision 1.2  2007/02/01 12:00:55  tim
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
		/// Stores the next ID to be assigned to the next user to log in
		/// </summary>
		private static int nextUserID = 0;
		/// <summary>
		/// The thread responsible for watching for incoming connections
		/// </summary>
		public static Thread listeningThread = new Thread(new ThreadStart(listeningThreadFunc));

		/// <summary>
		/// Constructor for ServerNetwork
		/// </summary>
		public ServerNetwork()
		{
		}

		/// <summary>
		/// Start listening for incoming connections
		/// </summary>
		public static void startListening()
		{
			listeningThread.Start();
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

		private static void listeningThreadFunc()
		{
			TcpListener listener = new TcpListener(IPAddress.Any, listeningPort);
			listener.Start();
			while(true)
			{
				while (listener.Pending() == false)
				{if (frmServer.ShuttingDown)
					  return;
					 Thread.Sleep(100);}
				TcpClient client = listener.AcceptTcpClient();

				connection conn = new connection();
				conn.userID = nextUserID;
				nextUserID++;
				conn.stream = client.GetStream();
				netStreams.Add(conn);
				conn.watchingClass = new ConnectionWatcher(conn);
			}
		}
	}
}

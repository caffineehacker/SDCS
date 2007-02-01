/* $Id$
 * $Log$
 * Revision 1.1  2007/02/01 14:05:15  tim
 * Started adding some database code for the server to keep track of users
 *
 */
using System;
using System.Timers;

namespace Server
{
	/// <summary>
	/// Summary description for ServerDatabase.
	/// </summary>
	public class ServerDatabase
	{
		private static UserDatabase database = new UserDatabase();
		public static bool DatabaseLoaded = false;

		private static Timer saveTimer = new Timer();

		public ServerDatabase()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Connects to the database at the default location. Only call this function once.
		/// </summary>
		/// <returns>True if loading is successful. False if the database is already loaded or loading failed.</returns>
		public static bool loadDatabase()
		{
			if (DatabaseLoaded)
				return false;
			database = new UserDatabase();
			try
			{
				database.ReadXml("userDatabase.xml");
				saveTimer.AutoReset = true;
				saveTimer.Interval = 600000;
				saveTimer.Elapsed +=new ElapsedEventHandler(saveTimer_Elapsed);
				saveTimer.Start();
			}
			catch
			{
				return false;
			}

			return true;
		}

		public static void saveDatabase()
		{
			database.AcceptChanges();
			database.WriteXml("userDatabase.xml");
		}

		private static void saveTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			saveDatabase();
		}
	}
}

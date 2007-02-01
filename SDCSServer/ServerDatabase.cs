/* $Id$
 * $Log$
 * Revision 1.3  2007/02/01 16:30:14  tim
 * Cleaned up some database code
 *
 * Revision 1.2  2007-02-01 16:19:41  tim
 * Added code for storing the user's data and adding a new user from the server program.
 *
 * Revision 1.1  2007-02-01 14:05:15  tim
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

		/// <summary>
		/// Timer to save the database periodically
		/// </summary>
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
				// Some last minute configuring that can't be done in the designer
				database.Users.UserIDColumn.AutoIncrement = true;
				database.Users.UserIDColumn.AllowDBNull = false;
				database.Users.UserIDColumn.Unique = true;
				database.UserData.UsernameColumn.Unique = true;
				database.Users.UserIDColumn.AllowDBNull = false;

				database.ReadXml("userDatabase.xml");
				saveTimer.AutoReset = true;
				saveTimer.Interval = 600000;
				saveTimer.Elapsed +=new ElapsedEventHandler(saveTimer_Elapsed);
				saveTimer.Start();
			}
			catch
			{
				DatabaseLoaded = false;
				return false;
			}

			DatabaseLoaded = true;
			return true;
		}

		/// <summary>
		/// Saves the database to disk
		/// </summary>
		public static void saveDatabase()
		{
			database.AcceptChanges();
			database.WriteXml("userDatabase.xml");
		}

		private static void saveTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			saveDatabase();
		}

		/// <summary>
		/// Adds a new user. Make sure you have loaded the database before trying to add a user.
		/// </summary>
		/// <param name="userName">The user name that you are adding</param>
		/// <param name="password">The password of the user you are adding</param>
		/// <returns>Returns true if the user was successfully added. Returns false otherwise.</returns>
		public static bool addUser(string userName, string password)
		{
			if (DatabaseLoaded == false)
				return false;

			database.AcceptChanges();
			try
			{
				UserDatabase.UsersRow newUserRow = database.Users.NewUsersRow();
				database.Users.AddUsersRow(newUserRow);
				UserDatabase.UserDataRow newUserDataRow = database.UserData.NewUserDataRow();
				newUserDataRow.Password = SDCSCommon.CryptoFunctions.getMD5Hash(password);
				newUserDataRow.Username = userName;
				newUserDataRow.UserID = newUserRow.UserID;
				database.UserData.AddUserDataRow(newUserDataRow);
			}
			catch
			{
				database.RejectChanges();
				return false;
			}
			database.AcceptChanges();
			return true;
		}
	}
}

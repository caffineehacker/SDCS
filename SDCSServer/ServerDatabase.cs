using System;
using System.Timers;

namespace Server
{
	/// <summary>
	/// This class provides functions for working with the database
	/// </summary>
	public class ServerDatabase
	{
		/// <summary>
		/// The database instance that will hold all the working information
		/// </summary>
		private static UserDatabase database = new UserDatabase();

		/// <summary>
		/// <see cref="DatabaseLoaded"/>
		/// </summary>
		private static bool databaseLoaded = false;
		/// <summary>
		/// Determines if the database has been loaded
		/// </summary>
		/// <value>True if loaded, false otherwise</value>
		public static bool DatabaseLoaded
		{
			set
			{
				databaseLoaded = value;
			}
		}

		/// <summary>
		/// Timer to save the database periodically
		/// </summary>
		private static Timer saveTimer = new Timer();

		/// <summary>
		/// Standard constructor
		/// </summary>
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
			if (databaseLoaded)
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

			try
			{
				database.ReadXml("userDatabase.xml");
			}
			catch
			{}

			DatabaseLoaded = true;

			foreach (UserDatabase.UserDataRow dat in database.UserData.Rows)
			{
				SDCSCommon.Network.BuddyListData bld = new SDCSCommon.Network.BuddyListData();
				bld.userID = dat.UserID;
				bld.username = dat.Username;
				bld.userState = SDCSCommon.Network.UserState.Offline;

				lock (ServerNetwork.buddyStates.SyncRoot)
					ServerNetwork.buddyStates.Add(dat.UserID, bld);
			}

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
		/// <param name="username">The user name that you are adding</param>
		/// <param name="password">The password of the user you are adding</param>
		/// <returns>Returns true if the user was successfully added. Returns false otherwise.</returns>
		public static bool addUser(string username, string password)
		{
			if (databaseLoaded == false)
				return false;

			database.AcceptChanges();
			try
			{
				UserDatabase.UsersRow newUserRow = database.Users.NewUsersRow();
				database.Users.AddUsersRow(newUserRow);
				UserDatabase.UserDataRow newUserDataRow = database.UserData.NewUserDataRow();
				newUserDataRow.Password = SDCSCommon.CryptoFunctions.getMD5Hash(password);
				newUserDataRow.Username = username;
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

		/// <summary>
		/// Gets the hashed version of a user's password
		/// </summary>
		/// <param name="username">The username whom's password you want</param>
		/// <returns>The user's hashed password</returns>
		public static string getUserPass(string username)
		{
			UserDatabase.UserDataRow userRow = database.UserData.FindByUsername(username);
			if (userRow == null)
				return "";
			else
				return userRow.Password;
		}

		/// <summary>
		/// Returns the user ID of the user defined in the passed string
		/// </summary>
		/// <param name="username">Username of the user who's user ID is need</param>
		/// <returns>The user ID for username if found, -1 otherwise</returns>
		public static int getUserID(string username)
		{
			UserDatabase.UserDataRow userRow = database.UserData.FindByUsername(username);
			if (userRow == null)
				return -1;
			else
				return userRow.UserID;
		}
	}
}

using System;
using System.Collections;

namespace SDCSCommon
{
	/// <summary>
	/// Contains generic functions and definitions for network communication that is needed by both the client and the server
	/// </summary>
	public class Network
	{
		// These constants are for network communication
		/// <summary>
		/// This is the header size for transmissions in bytes
		/// </summary>
		public const int HEADER_SIZE = 17;
		
		/// <summary>
		/// Sent by the server when login information checks out
		/// </summary>
		public const int LoginOK = 0;
		/// <summary>
		/// Sent by the server when login information is bad
		/// </summary>
		public const int LoginBad = 1;

		/// <summary>
		/// This is the structure for the header
		/// </summary>
		/// <remarks>When changing this make sure to change the HEADER_SIZE and the headerToBytes and bytesToHeader functions</remarks>
		public struct Header
		{
			/// <summary>
			/// ID that the data is being sent from
			/// </summary>
			public int FromID;
			/// <summary>
			/// ID that the data is being sent to
			/// </summary>
			public int ToID;
			/// <summary>
			/// The data type of the data being sent
			/// </summary>
			public DataTypes DataType;
			/// <summary>
			/// If true it means that data has been encrypted using the user's single hashed password and a symmetrical encryption algorithm
			/// </summary>
			public bool Encrypted;
			/// <summary>
			/// The length of the data not including this header
			/// </summary>
			public int Length;
		}

		/// <summary>
		/// This enumeration creates a different value for each data type
		/// </summary>
		public enum DataTypes
		{
			/// <summary>
			/// For Instant Message data
			/// </summary>
			InstantMessage,
			/// <summary>
			/// For White Board data
			/// </summary>
			WhiteBoard,
			/// <summary>
			/// For creating the double hashed password code.
			/// </summary>
			RandomPassCode,
			/// <summary>
			/// Sent from the client with the Username and double hashed password
			/// </summary>
			LoginInformation,
			/// <summary>
			/// Sent from the server to notify the client if the login was successful or not
			/// </summary>
			LoginStatus,
			/// <summary>
			/// Sent from the server with updated information on buddy states
			/// </summary>
			BuddyListUpdate,
			/// <summary>
			/// Sent from the client to the server in order to update the logged in user's directory listing
			/// </summary>
			DirectoryUpdate,
			/// <summary>
			/// Sent from the client to request the directory info on another user
			/// </summary>
			DirectoryRequest,
			/// <summary>
			/// Sent by the server or the client to determine if the network connection is still alive
			/// </summary>
			Ping
		}

		/// <summary>
		/// Used to convey the state a user is in through buddy list data
		/// </summary>
		public enum UserState
		{
			/// <summary>
			/// The user has gone offline
			/// </summary>
			Offline,
			/// <summary>
			/// The user has come online
			/// </summary>
			Online
		}

		/// <summary>
		/// A structure for holding data about buddy list updates
		/// </summary>
		public class BuddyListData
		{
			/// <summary>
			/// The user ID of the buddy for which the status is being updated
			/// </summary>
			public int userID;
			/// <summary>
			/// The username of the buddy
			/// </summary>
			public string username;
			/// <summary>
			/// The new state of the buddy
			/// </summary>
			public UserState userState;

			/// <summary>
			/// A generic object for misc data
			/// </summary>
			public object Tag = null;

			/// <summary>
			/// Returns the username of this buddy
			/// </summary>
			/// <returns>This buddy's username</returns>
			public override string ToString()
			{
				return username;
			}

		}

		/// <summary>
		/// Sent as data from a client when updating their directory listing
		/// </summary>
		public struct DirectoryUpdateData
		{
			// ToDo: Add fields for each piece of information that will be in the directory
		}

		/// <summary>
		/// Sent from the server in response to a directory request
		/// </summary>
		public struct DirectoryRequestData
		{
			// ToDo: Add fields for each piece of information that will be in the directory
		}

		/// <summary>
		/// Standard empty constructor
		/// </summary>
		public Network()
		{
		}

		/// <summary>
		/// Converts a Header in to an array of bytes for transmission over the network
		/// </summary>
		/// <param name="head">The header to be converted</param>
		/// <returns>A byte array representing the header</returns>
		/// <example>Assuming we want to send an instant message
		/// <code>SDCSCommon.Network.Header h = new SDCSCommon.Network.Header();
		/// h.ToID = 3;
		/// h.FromID = 0;
		/// h.DataType = SDCSCommon.Network.DataTypes.InstantMessage;
		/// h.Encrypted = false;
		/// h.Length = Data.Length; // Assuming data is an array of bytes representing the IM text
		/// 
		/// netStream.Write(headerToBytes(h), 0, SDCSCommon.Network.HEADER_SIZE);</code></example>
		public static byte[] headerToBytes(Header head)
		{
			ArrayList temp = new ArrayList();
			temp.AddRange(System.BitConverter.GetBytes(head.FromID));
			temp.AddRange(System.BitConverter.GetBytes(head.ToID));
			temp.AddRange(System.BitConverter.GetBytes((int)head.DataType));
			temp.AddRange(System.BitConverter.GetBytes(head.Encrypted));
			temp.AddRange(System.BitConverter.GetBytes(head.Length));
			return (byte[])temp.ToArray(typeof(byte));
		}

		/// <summary>
		/// Converts an array of bytes to a Header
		/// </summary>
		/// <param name="bytes">The bytes we're converting from</param>
		/// <returns>The Header that the bytes coded for</returns>
		public static Header bytesToHeader(byte[] bytes)
		{
			Header temp = new Header();
			temp.FromID = System.BitConverter.ToInt32(bytes, 0);
			temp.ToID = System.BitConverter.ToInt32(bytes, 4);
			temp.DataType = (DataTypes)System.BitConverter.ToInt32(bytes, 8);
			temp.Encrypted = System.BitConverter.ToBoolean(bytes, 12);
			temp.Length = System.BitConverter.ToInt32(bytes, 13);

			return temp;
		}

		/// <summary>
		/// Converts login information in the form of a username and password to a format to be sent across the network
		/// </summary>
		/// <param name="username">The username to convert</param>
		/// <param name="password">The double hashed password to convert</param>
		/// <returns>The bytes to be sent</returns>
		public static byte[] loginInformationToData(string username, string password)
		{
			ArrayList data = new ArrayList();
			byte[] usernameBytes = System.Text.UnicodeEncoding.Unicode.GetBytes(username);
			data.AddRange(System.BitConverter.GetBytes(usernameBytes.Length));
			data.AddRange(usernameBytes);
			data.AddRange(System.Text.UnicodeEncoding.Unicode.GetBytes(password));

			return (byte[])data.ToArray(typeof(byte));
		}

		/// <summary>
		/// Converts the byte array received as data for logging in to the username and hashed password format
		/// </summary>
		/// <param name="data">The data received to be converted</param>
		/// <returns>An array with [0] being the username and [1] being the hashed password</returns>
		public static string[] dataToLoginInformation(byte[] data)
		{
			int usernameLength = System.BitConverter.ToInt32(data, 0);
			string[] returnVal = new string[2];
			returnVal[0] = System.Text.UnicodeEncoding.Unicode.GetString(data, 4, usernameLength);
			returnVal[1] = System.Text.UnicodeEncoding.Unicode.GetString(data, 4 + usernameLength, data.Length - (4 + usernameLength));

			return returnVal;
		}

		/// <summary>
		/// Converts BuddyListData[] into bytes to be sent across the network
		/// </summary>
		/// <param name="data">The BuddyListData to convert</param>
		/// <returns>Bytes representing the given BuddyListData</returns>
		public static byte[] BuddyListDataToBytes(BuddyListData[] data)
		{
			ArrayList temp = new ArrayList();

			for (int i = 0; i < data.Length; i++)
			{
				byte[] usernameBytes = System.Text.UnicodeEncoding.Unicode.GetBytes(data[i].username);
				temp.AddRange(System.BitConverter.GetBytes(data[i].userID));
				temp.AddRange(System.BitConverter.GetBytes(usernameBytes.Length));
				temp.AddRange(usernameBytes);
				temp.AddRange(System.BitConverter.GetBytes((int)data[i].userState));
			}
			return (byte[])temp.ToArray(typeof(byte));
		}

		/// <summary>
		/// Converts bytes received as data from the server in to managable data
		/// </summary>
		/// <param name="bytes">The bytes to be converted</param>
		/// <returns>Managable BuddyListData array</returns>
		public static BuddyListData[] BytesToBuddyListData(byte[] bytes)
		{
			ArrayList returnVal = new ArrayList();

			int baseCount = 0;
			while (baseCount < bytes.Length)
			{
				BuddyListData data = new BuddyListData();
				data.userID = System.BitConverter.ToInt32(bytes,baseCount);
				int usernameLength = System.BitConverter.ToInt32(bytes, baseCount + 4);
				data.username = System.Text.UnicodeEncoding.Unicode.GetString(bytes, baseCount + 8, usernameLength);
				data.userState = (UserState)System.BitConverter.ToInt32(bytes, baseCount + 8 + usernameLength);
				baseCount += 12 + usernameLength;

				returnVal.Add(data);
			}

			return (BuddyListData[])returnVal.ToArray(typeof(BuddyListData));
		}
	}
}

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
		public const int HEADER_SIZE = 20;

		/// <summary>
		/// This is the structure for the header
		/// </summary>
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
			/// The length of the data not including this header
			/// </summary>
			public Int64 Length;
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
			/// For assigning a client a user ID. Is this necessary?
			/// </summary>
			UserIDAssignment,
			/// <summary>
			/// For sending a random code that is to be combined with a users password for logging in securely
			/// </summary>
			RandomPassCode,
			/// <summary>
			/// Sent from the client with the Username and double hashed password
			/// </summary>
			LoginInformation,
			/// <summary>
			/// Sent from the server with updated information on buddy states
			/// </summary>
			BuddyListUpdate
		}

		public Network()
		{
		}

		/// <summary>
		/// Converts a Header in to an array of bytes for transmission over the network
		/// </summary>
		/// <param name="head">The header to be converted</param>
		/// <returns>A byte array representing the header</returns>
		public static byte[] headerToBytes(Header head)
		{
			ArrayList temp = new ArrayList();
			temp.AddRange(System.BitConverter.GetBytes(head.FromID));
			temp.AddRange(System.BitConverter.GetBytes(head.ToID));
			temp.AddRange(System.BitConverter.GetBytes((int)head.DataType));
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
			temp.Length = System.BitConverter.ToInt64(bytes, 12);

			return temp;
		}
	}
}

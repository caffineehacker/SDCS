/* $Id$
 * $Log$
 * Revision 1.4  2007/02/04 05:28:53  tim
 * Updated all of the XML comments
 *
 * Revision 1.3  2007-02-01 16:19:41  tim
 * Added code for storing the user's data and adding a new user from the server program.
 *
 * Revision 1.2  2007-02-01 12:00:55  tim
 * Added CVS keywords
 *
 */

using System;
using System.Text;
using System.Security.Cryptography;

namespace SDCSCommon
{
	/// <summary>
	/// Summary description for MD5.
	/// </summary>
	public class CryptoFunctions
	{
		/// <summary>
		/// Standard constructor
		/// </summary>
		public CryptoFunctions()
		{}

		/// <summary>
		/// One-way hashes a string using the MD5 algorithm
		/// </summary>
		/// <param name="str">String to apply the MD5 hash to</param>
		/// <returns>The MD5 hashed version of the passed string</returns>
		public static string getMD5Hash(string str)
		{
			// First we need to convert the string into bytes, which
			// means using a text encoder.
			Encoder enc = System.Text.Encoding.Unicode.GetEncoder();

			// Create a buffer large enough to hold the string
			byte[] unicodeText = new byte[str.Length * 2];
			enc.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);

			// Now that we have a byte array we can ask the CSP to hash it
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] result = md5.ComputeHash(unicodeText);

			// Build the final string by converting each byte
			// into hex and appending it to a StringBuilder
			StringBuilder sb = new StringBuilder();
			for (int i=0;i<result.Length;i++)
			{
				sb.Append(result[i].ToString("X2"));
			}

			// And return it
			return sb.ToString();
		}
	}
}

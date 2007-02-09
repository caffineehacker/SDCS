/* $Id$
 * $Log$
 * Revision 1.7  2007/02/09 18:08:47  tim
 * Updated documentation
 *
 * Revision 1.6  2007-02-09 22:01:03  tim
 * Clean up
 *
 * Revision 1.5  2007-02-09 21:52:22  tim
 * Added some code to prepare for encrypting data
 *
 * Revision 1.4  2007-02-04 05:28:53  tim
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
	/// Contains functions for facilitating cryptographic actions
	/// </summary>
	public class CryptoFunctions
	{
		/// <summary>
		/// This is to provide a default IV so that the cryptography functions have a seed
		/// </summary>
		/// <remarks>DO NOT CHANGE THIS VALUE</remarks>
		static byte[] InitializationVector = new byte[] {0x00, 0xFF, 0x55, 0xAA, 0x5A, 0xA5, 0x51, 0x67};

		/// <summary>
		/// Standard empty constructor
		/// </summary>
		public CryptoFunctions()
		{}

		/// <summary>
		/// One-way hashes a string using the MD5 algorithm
		/// </summary>
		/// <param name="str">String to apply the MD5 hash to</param>
		/// <returns>The MD5 hashed version of the passed string</returns>
		/// <example>If you were testing a newly entered password against its stored hase:
		/// if (getMD5Hash(newPassword) != storedHash)
		///		MessageBox.Show("Invalid Password");</example>
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

		/// <summary>
		/// Encrypts the given bytes using a symmetrical encryption algorithm with encryptionCode as the key
		/// </summary>
		/// <param name="toEncrypt">The bytes to be encrypted</param>
		/// <param name="encryptionCode">The key to be used for encrypting/decrypting</param>
		/// <returns>The encoded bytes</returns>
		public static byte[] EncryptBytes(byte[] toEncrypt, string encryptionCode)
		{
			RijndaelManaged rij = new RijndaelManaged();
			rij.Key = System.Text.UnicodeEncoding.Unicode.GetBytes(encryptionCode);
			rij.IV = InitializationVector;

			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			CryptoStream cs = new CryptoStream(ms, rij.CreateEncryptor(), CryptoStreamMode.Write);
			cs.Write(toEncrypt, 0, toEncrypt.Length);
			cs.Close();
			ms.Close();

			return ms.ToArray();
		}

		/// <summary>
		/// Decrypts data using a symmetrical encryption technique
		/// </summary>
		/// <param name="toDecrypt">The bytes to be decrypted</param>
		/// <param name="encryptionCode">The key for decrypting</param>
		/// <returns>The decrypted bytes</returns>
		public static byte[] DecryptBytes(byte[] toDecrypt, string encryptionCode)
		{
			RijndaelManaged rij = new RijndaelManaged();
			rij.Key = System.Text.UnicodeEncoding.Unicode.GetBytes(encryptionCode);
			rij.IV = InitializationVector;

			System.IO.MemoryStream ms = new System.IO.MemoryStream(toDecrypt, 0, toDecrypt.Length);
			CryptoStream cs = new CryptoStream(ms, rij.CreateEncryptor(), CryptoStreamMode.Read);

			byte[] data = new byte[toDecrypt.Length];
			cs.Read(data, 0, toDecrypt.Length);
			cs.Close();

			return data;
		}
	}
}

/*
 * This class was obtained from:
 * http://www.dijksterhuis.org/creating-salted-hash-values-in-c/
 * 
 * */
 
using System;
using System.Security.Cryptography;
using System.Text;

namespace SaltedHash
{
    class SaltedHash
    {
        HashAlgorithm HashProvider;
        int SalthLength;

        /// <summary>
        /// The constructor takes a HashAlgorithm as a parameter.
        /// </summary>
        /// <param name="HashAlgorithm">
        /// A <see cref="HashAlgorithm"/> HashAlgorihm which is derived from HashAlgorithm. C# provides
        /// the following classes: SHA1Managed,SHA256Managed, SHA384Managed, SHA512Managed and MD5CryptoServiceProvider
        /// </param>

        public SaltedHash(HashAlgorithm HashAlgorithm, int theSaltLength)
        {
            HashProvider = HashAlgorithm;
            SalthLength = theSaltLength;
        }

        /// <summary>
        /// Default constructor which initialises the SaltedHash with the SHA256Managed algorithm
        /// and a Salt of 32 bytes ( or 32*8 = 256 bits)
        /// </summary>

        public SaltedHash() : this(new SHA256Managed(), 32)
        {
        }

        /// <summary>
        /// The actual hash calculation is shared by both GetHashAndSalt and the VerifyHash functions
        /// </summary>
        /// <param name="Data">A byte array of the Data to Hash</param>
        /// <param name="Salt">A byte array of the Salt to add to the Hash</param>
        /// <returns>A byte array with the calculated hash</returns>

        private byte[] ComputeHash(byte[] Data, byte[] Salt)
        {
            // Allocate memory to store both the Data and Salt together
            byte[] DataAndSalt = new byte[Data.Length + SalthLength];

            // Copy both the data and salt into the new array
            Array.Copy(Data, DataAndSalt, Data.Length);
            Array.Copy(Salt, 0, DataAndSalt, Data.Length, SalthLength);

            // Calculate the hash
            // Compute hash value of our plain text with appended salt.
            return HashProvider.ComputeHash(DataAndSalt);
        }

        /// <summary>
        /// Given a data block this routine returns both a Hash and a Salt
        /// </summary>
        /// <param name="Data">
        /// A <see cref="System.Byte"/>byte array containing the data from which to derive the salt
        /// </param>
        /// <param name="Hash">
        /// A <see cref="System.Byte"/>byte array which will contain the hash calculated
        /// </param>
        /// <param name="Salt">
        /// A <see cref="System.Byte"/>byte array which will contain the salt generated
        /// </param>

        public void GetHashAndSalt(byte[] Data, out byte[] Hash, out byte[] Salt)
        {
            // Allocate memory for the salt
            Salt = new byte[SalthLength];

            // Strong runtime pseudo-random number generator, on Windows uses CryptAPI
            // on Unix /dev/urandom
            RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();

            // Create a random salt
            random.GetNonZeroBytes(Salt);

            // Compute hash value of our data with the salt.
            Hash = ComputeHash(Data, Salt);
        }

        /// <summary>
        /// The routine provides a wrapper around the GetHashAndSalt function providing conversion
        /// from the required byte arrays to strings. Both the Hash and Salt are returned as Base-64 encoded strings.
        /// </summary>
        /// <param name="Data">
        /// A <see cref="System.String"/> string containing the data to hash
        /// </param>
        /// <param name="Hash">
        /// A <see cref="System.String"/> base64 encoded string containing the generated hash
        /// </param>
        /// <param name="Salt">
        /// A <see cref="System.String"/> base64 encoded string containing the generated salt
        /// </param>

        public void GetHashAndSaltString(string Data, out string Hash, out string Salt)
        {
            byte[] HashOut;
            byte[] SaltOut;

            // Obtain the Hash and Salt for the given string
            GetHashAndSalt(Encoding.UTF8.GetBytes(Data), out HashOut, out SaltOut);

            // Transform the byte[] to Base-64 encoded strings
            Hash = Convert.ToBase64String(HashOut);
            Salt = Convert.ToBase64String(SaltOut);
        }

        /// <summary>
        /// This routine verifies whether the data generates the same hash as we had stored previously
        /// </summary>
        /// <param name="Data">The data to verify </param>
        /// <param name="Hash">The hash we had stored previously</param>
        /// <param name="Salt">The salt we had stored previously</param>
        /// <returns>True on a succesfull match</returns>

        public bool VerifyHash(byte[] Data, byte[] Hash, byte[] Salt)
        {
            byte[] NewHash = ComputeHash(Data, Salt);

            //  No easy array comparison in C# -- we do the legwork
            if (NewHash.Length != Hash.Length) return false;

            for (int Lp = 0; Lp < Hash.Length; Lp++ )
                if (!Hash[Lp].Equals(NewHash[Lp]))
                    return false;

            return true;
        }

        /// <summary>
        /// This routine provides a wrapper around VerifyHash converting the strings containing the
        /// data, hash and salt into byte arrays before calling VerifyHash.
        /// </summary>
        /// <param name="Data">A UTF-8 encoded string containing the data to verify</param>
        /// <param name="Hash">A base-64 encoded string containing the previously stored hash</param>
        /// <param name="Salt">A base-64 encoded string containing the previously stored salt</param>
        /// <returns></returns>

        public bool VerifyHashString(string Data, string Hash, string Salt)
        {
            byte[] HashToVerify = Convert.FromBase64String(Hash);
            byte[] SaltToVerify = Convert.FromBase64String(Salt);
            byte[] DataToVerify = Encoding.UTF8.GetBytes(Data);
            return VerifyHash(DataToVerify, HashToVerify, SaltToVerify);
        }

    }

    /*
    /// <summary>
    /// This little demo code shows how to encode a users password.
    /// </summary>

    class SaltedHashDemo
    {
        public static void Main(string[] args)
        {
            // We use the default SHA-256 & 4 byte length
            SaltedHash demo = new SaltedHash();

            // We have a password, which will generate a Hash and Salt
            string Password = "MyGlook234";
            string Hash;
            string Salt;

            demo.GetHashAndSaltString(Password, out Hash, out Salt);
            Console.WriteLine("Password = {0} , Hash = {1} , Salt = {2}", Password, Hash, Salt);

            // Password validation
            //
            // We need to pass both the earlier calculated Hash and Salt (we need to store this somewhere safe between sessions)

            // First check if a wrong password passes
            string WrongPassword = "OopsOops";
            Console.WriteLine("Verifying {0} = {1}", WrongPassword, demo.VerifyHashString(WrongPassword, Hash, Salt));

            // Check if the correct password passes
            Console.WriteLine("Verifying {0} = {1}", Password, demo.VerifyHashString(Password, Hash, Salt));

        }
    }
    */

}

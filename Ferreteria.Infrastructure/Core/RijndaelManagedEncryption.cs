using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Ferreteria.Infrastructure.Core
{
    public static class RijndaelManagedEncryption
    {
        private const string InputKey = "30811BF8-E9DD-4419-8EAB-8301636B612C";

        public static string DecryptRijndael(string cipherText, string salt)
        {
            string end;
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentNullException("cipherText");
            }
            if (!RijndaelManagedEncryption.IsBase64String(cipherText))
            {
                throw new Exception("The cipherText input parameter is not base64 encoded");
            }
            RijndaelManaged rijndaelManaged = RijndaelManagedEncryption.NewRijndaelManaged(salt);
            ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
            using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        end = streamReader.ReadToEnd();
                    }
                }
            }
            return end;
        }

        public static string EncryptRijndael(string text, string salt)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }
            RijndaelManaged rijndaelManaged = RijndaelManagedEncryption.NewRijndaelManaged(salt);
            ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV);
            MemoryStream memoryStream = new MemoryStream();
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
            {
                using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                {
                    streamWriter.Write(text);
                }
            }
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        private static bool IsBase64String(string base64String)
        {
            base64String = base64String.Trim();
            if (base64String.Length % 4 != 0)
            {
                return false;
            }
            return Regex.IsMatch(base64String, "^[a-zA-Z0-9\\+/]*={0,3}$", RegexOptions.None);
        }

        private static RijndaelManaged NewRijndaelManaged(string salt)
        {
            if (salt == null)
            {
                throw new ArgumentNullException("salt");
            }
            byte[] bytes = Encoding.ASCII.GetBytes(salt);
            Rfc2898DeriveBytes rfc2898DeriveByte = new Rfc2898DeriveBytes("30811BF8-E9DD-4419-8EAB-8301636B612C", bytes);

            RijndaelManaged rijndaelManaged = new RijndaelManaged();

            rijndaelManaged.Key = rfc2898DeriveByte.GetBytes(rijndaelManaged.KeySize / 8);
            rijndaelManaged.IV = rfc2898DeriveByte.GetBytes(rijndaelManaged.BlockSize / 8);

            return rijndaelManaged;
        }
    }
}

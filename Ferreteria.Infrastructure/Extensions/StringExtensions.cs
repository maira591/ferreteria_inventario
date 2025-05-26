using Ferreteria.Infrastructure.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Ferreteria.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        private readonly static byte[] EncryptionKey;

        static StringExtensions()
        {
            EncryptionKey = Encoding.ASCII.GetBytes("9PAEu83G");
        }

        public static string Compress(this string value)
        {
            string base64String;
            using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(value)))
            {
                MemoryStream memoryStream1 = new MemoryStream();
                using (GZipStream gZipStream = new GZipStream(memoryStream1, CompressionMode.Compress))
                {
                    memoryStream.CopyTo(gZipStream);
                }

                base64String = Convert.ToBase64String(memoryStream1.ToArray());
            }

            return base64String;
        }

        public static string Decompress(this string value)
        {
            string str;
            byte[] numArray = Convert.FromBase64String(value);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream gZipStream = new GZipStream(new MemoryStream(numArray), CompressionMode.Decompress))
                {
                    gZipStream.CopyTo(memoryStream);
                }

                str = Encoding.Unicode.GetString(memoryStream.ToArray());
            }

            return str;
        }

        private static int GetHexVal(char hex)
        {
            char chr = hex;
            return chr - (chr < ':' ? '0' : '7');
        }

        public static string GetNotNull(this string value)
        {
            return value ?? string.Empty;
        }

        public static byte[] HexToByte(this string value)
        {
            string upper = value.Substring(2, value.Length - 2).ToUpper();
            if (upper.Length % 2 == 1)
            {
                throw new Exception("The binary key cannot have an odd number of digits");
            }

            int length = upper.Length / 2;
            byte[] hexVal = new byte[length];
            for (int i = 0; i < length; i++)
            {
                hexVal[i] = (byte)((StringExtensions.GetHexVal(upper[i << 1]) << 4) +
                                    StringExtensions.GetHexVal(upper[(i << 1) + 1]));
            }

            return hexVal;
        }

        public static string Inject(this string format, params object[] formattingArgs)
        {
            return string.Format(format, formattingArgs);
        }

        public static string Inject(this string format, params string[] formattingArgs)
        {
            return string.Format(format, (
                from a in formattingArgs
                select a).ToArray<object>());
        }

        public static bool IsEmpty(this string value)
        {
            return value == string.Empty;
        }

        public static bool IsEmptyOrNull(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static string RemoveAtRight(this string value, int count)
        {
            return value.Substring(0, value.Length - count);
        }

        public static string Surround(this string value, string delimiter)
        {
            return string.Format("{0}{1}{0}", delimiter, value);
        }

        public static bool ToBool(this string value)
        {
            if (value == "0")
            {
                return false;
            }

            if (value == "1")
            {
                return true;
            }

            return bool.Parse(value);
        }

        public static DateTime ToDate(this string value)
        {
            return DateTime.Parse(value, CultureInfo.InvariantCulture);
        }

        public static Guid ToGuid(this string value)
        {
            return Guid.Parse(value);
        }

        public static int ToInt(this string value)
        {
            return int.Parse(value);
        }

        public static long ToLong(this string value)
        {
            return long.Parse(value);
        }

        public static string ToString(this string @this, params KeyValuePair<string, string>[] pairs)
        {
            if (string.IsNullOrEmpty(@this))
            {
                return @this;
            }

            return ((IEnumerable<KeyValuePair<string, string>>)pairs).Aggregate<KeyValuePair<string, string>, string>(
                @this, (string current, KeyValuePair<string, string> pair) => current.Replace(pair.Key, pair.Value));
        }


        public static string XmlDecode(this string value)
        {
            if (value.IsEmptyOrNull())
            {
                return value;
            }

            return value.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&amp;", "&");
        }

        public static string EncryptAes(this string @this, string key)
        {
            return RijndaelManagedEncryption.EncryptRijndael(@this, key);
        }

        public static string DecryptAes(this string @this, string key)
        {
            return RijndaelManagedEncryption.DecryptRijndael(@this, key);
        }
    }
}

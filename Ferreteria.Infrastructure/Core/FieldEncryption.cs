using Ferreteria.Infrastructure.Extensions;

namespace Ferreteria.Infrastructure.Core
{
    public static class FieldEncryption
    {
        /// <summary> Encrypt a field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string Encrypt(string field)
        {
            return field.EncryptAes(Encryption.Salt);
        }

        /// <summary> Decrypt a field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string Decrypt(string field)
        {
            return field.DecryptAes(Encryption.Salt);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Ferreteria.Infrastructure.Common
{
    public class ConvertService : IConvertService
    {
        /// <summary>
        /// Convierte el ID HEXADECIMAL almacenado en Oracle a un valor GUID
        /// </summary>
        /// <param name="hexadecimalValue"></param>
        /// <returns></returns>
        public Guid ConvertHexadecimalToGuid(string hexadecimalValue)
        {
            var ba = StrToByteArray(hexadecimalValue);

            var strReturn = new StringBuilder(ba.Length * 2);
            strReturn.AppendFormat("{0:x2}", ba[3]);
            strReturn.AppendFormat("{0:x2}", ba[2]);
            strReturn.AppendFormat("{0:x2}", ba[1]);
            strReturn.AppendFormat("{0:x2}", ba[0]);
            strReturn.AppendFormat("{0:x2}", ba[5]);
            strReturn.AppendFormat("{0:x2}", ba[4]);
            strReturn.AppendFormat("{0:x2}", ba[7]);
            strReturn.AppendFormat("{0:x2}", ba[6]);

            for (int i = 8; i < ba.Length; i++) strReturn.AppendFormat("{0:x2}", ba[i]);

            return Guid.Parse(strReturn.ToString());
        }

        /// <summary>
        /// Convierte una cadena a un byte[]
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] StrToByteArray(string str)
        {
            var hexindex = new Dictionary<string, byte>();
            for (int i = 0; i <= 255; i++)
                hexindex.Add(i.ToString("X2"), (byte)i);

            var hexres = new List<byte>();
            for (int i = 0; i < str.Length; i += 2)
                hexres.Add(hexindex[str.Substring(i, 2)]);

            return hexres.ToArray();
        }
    }
}

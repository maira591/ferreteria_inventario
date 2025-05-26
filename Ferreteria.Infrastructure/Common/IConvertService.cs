using System;

namespace Ferreteria.Infrastructure.Common
{
    public interface IConvertService
    {
        Guid ConvertHexadecimalToGuid(string hexadecimalValue);
        byte[] StrToByteArray(string str);
    }
}

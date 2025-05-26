using MailKit.Security;

namespace Ferreteria.Domain.ViewModel.Email
{
    public class MailServerVM
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public SecureSocketOptions SecureSocketOptions { get; set; }
    }
}

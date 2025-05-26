using System.Collections.Generic;

namespace Ferreteria.Domain.ViewModel.Email
{
    public class EmailMessageVM
    {
        public string Password { get; set; }
        public MailServerVM Server { get; set; }
        public string Subject { get; set; }
        public FromEmailVM FromEmail { get; set; }
        public IEnumerable<ToEmailVM> ToEmail { get; set; }
        public string Body { get; set; }
        public IEnumerable<string> Attachment { get; set; }
    }
}

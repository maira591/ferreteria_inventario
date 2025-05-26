namespace Ferreteria.Domain.ViewModel.Notification
{
    public class RequestEmailVM
    {
        public string From { get; set; }
        public string[] To { get; set; }
        public string[] Cc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtmlBody { get; set; }
        public string[] AttachmentPaths { get; set; }
    }
}
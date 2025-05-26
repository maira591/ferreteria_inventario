using System.Net;

namespace Ferreteria.Domain.ViewModel.Auth
{
    public class ResponseVM<T> where T : class
    {
        public HttpStatusCode Status { get; set; }
        public T Body { get; set; }
        public string Message { get; set; }
        public string Code { get; set; }
    }
}
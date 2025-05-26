using Ferreteria.Domain.ViewModel.Login;

namespace Ferreteria.Domain.ViewModel.Auth
{
    public class RequestVM<T>
    {
        public HeaderVM Header { get; set; }
        public T Body { get; set; }
    }
}
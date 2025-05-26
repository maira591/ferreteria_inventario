namespace Ferreteria.Domain.ViewModel.Login
{
    public class BodyVM<T> where T : class
    {
        public T CurrentBody { get; set; }
    }
}
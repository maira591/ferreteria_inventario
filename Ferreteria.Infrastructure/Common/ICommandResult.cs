namespace Ferreteria.Infrastructure.Common
{
    public interface ICommandResult
    {
        string Code
        {
            get;
            set;
        }

        string Message
        {
            get;
            set;
        }

        bool Success
        {
            get;
            set;
        }
    }
}

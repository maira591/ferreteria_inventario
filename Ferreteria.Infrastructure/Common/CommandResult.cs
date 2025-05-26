namespace Ferreteria.Infrastructure.Common
{
    public class CommandResult : ICommandResult
    {
        public string Code
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public bool Success
        {
            get;
            set;
        }

    }
}

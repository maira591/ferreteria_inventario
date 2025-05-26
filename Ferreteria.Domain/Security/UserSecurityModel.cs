namespace Ferreteria.Domain.Security
{
    public class UserSecurityModel
    {
        public string _user { get; set; }
        public string _pass { get; set; }

        public UserSecurityModel(string user, string pass)
        {
            _user = user;
            _pass = pass;

        }
    }
}

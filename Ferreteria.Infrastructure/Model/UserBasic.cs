using System;

namespace Ferreteria.Infrastructure.Model
{
    public class UserBasic
    {
        #region Properties
        public string Token { get; set; }
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Organization { get; set; }
        public bool Enabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public Guid? LdapGuid { get; set; }
        public string Password { get; set; }
        public Guid RolId { get; set; }
        public Guid OrganizationId { get; set; }
        #endregion
    }
}

using Core.Application.Website.Utils;
using Core.DataAccess.Model.Generics;
using Core.Domain.ViewModel.Login;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [RegularExpression("^[^\\s]+$", ErrorMessage = "El Nombre de Usuario no puede contener espacios.")]
        [Display(Name = "Nombre de Usuario")]
        public string Username { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool Captcha { get; set; }
    }

    public interface IUserBasicModel
    {
        UserBasicModel GetCurrentUser();
        void SetCurrentUser(UserBasicModel userModel);
    }

    public class UserBasicModel : IUserBasicModel
    {
        private readonly IHttpContextAccessor _accessor;
        public UserBasicModel()
        {

        }
        public UserBasicModel(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        #region Properties
        public string Token { get; set; }
        public Guid Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [RegularExpression("^[^\\s]+$", ErrorMessage = "El Nombre de Usuario no puede contener espacios.")]
        [Display(Name = "Nombre de Usuario")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Nombres y Apellidos")]
        public string Name { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [EmailAddress(ErrorMessage = "El campo {0} no es un correo válido.")]
        [Display(Name = "Correo")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Phone]
        [Display(Name = "Teléfono")]
        public string PhoneNumber { get; set; }
        public List<RoleModel> Roles { get; set; }
        [Display(Name = "Cooperativa")]
        public string Organization { get; set; }
        [Display(Name = "Cooperativa")]
        public string OrganizationName { get; set; }
        [Display(Name = "Habilitado")]
        public bool Enabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public Guid? LdapGuid { get; set; }
        public string Password { get; set; }
        public Guid RolId { get; set; }
        public Guid OrganizationId { get; set; }
        /// <summary>
        /// Indica si aceptó el Habeas Data
        /// </summary>
        public bool AcceptedHabeasData { get; set; }
        /// <summary>
        /// Indica si se aceptó los terminos y condiciones
        /// </summary>
        public bool AcceptedTermsAndConditions { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Cargo")]
        public string Position { get; set; }
        #endregion

        #region Methods
        public UserBasicModel GetCurrentUser()
        {
            var pru = _accessor.HttpContext.Session.GetObject<UserBasicModel>("Identity");
            return pru;
        }
        public void SetCurrentUser(UserBasicModel userModel)
        {
            _accessor.HttpContext.Session.SetObject("Identity", userModel);
            _accessor.HttpContext.Session.SetInt32("timeoutSessionInMilliseconds", int.Parse(System.Configuration.ConfigurationManager.AppSettings["timeoutSessionInMilliseconds"]));
            _accessor.HttpContext.Session.SetInt32("timeWaitingForAnswerInactivitySeconds", int.Parse(System.Configuration.ConfigurationManager.AppSettings["timeWaitingForAnswerInactivitySeconds"]));
        }
        #endregion
    }

    public class RememberPassword
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [RegularExpression("^[^\\s]+$", ErrorMessage = "El Nombre de Usuario no puede contener espacios.")]
        [Display(Name = "Nombre de Usuario")]
        public string UserName { get; set; }
        public string Token { get; set; }

    }
    public class VerifyToken : RememberPassword
    {
        public Guid UserId { get; set; }
        public int TimeToLive { get; set; }
    }
    public class PasswordChange : VerifyToken
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirme Nueva Contraseña")]
        [Compare("NewPassword", ErrorMessage = "Contraseñas no coinciden, intente de nuevo!")]
        public string ConfirmNewPassword { get; set; }
        public PasswordPoliciesVM PasswordPolicies { get; set; }
    }
    public class ChangePassword : VerifyToken
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirme Nueva Contraseña")]
        [Compare("NewPassword", ErrorMessage = "Contraseñas no coinciden, intente de nuevo!")]
        public string ConfirmNewPassword { get; set; }
        public PasswordPoliciesVM PasswordPolicies { get; set; }
        public GenericResponse<string> response { get; set; }
    }
}
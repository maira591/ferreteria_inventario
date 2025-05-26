using Ferreteria.Application.Website.Utils;
using Ferreteria.DataAccess.Model.Generics;
using Ferreteria.Domain.ViewModel.Login;
using System.ComponentModel.DataAnnotations;

namespace Ferreteria.Application.Website.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [RegularExpression("^[^\\s]+$", ErrorMessage = "El Nombre de Usuario no puede contener espacios.")]
        [Display(Name = "Nombre de Usuario")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
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
        public int UserId { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [Display(Name = "Nombres y Apellidos")]
        public string Name { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [EmailAddress(ErrorMessage = "El campo {0} no es un correo válido.")]
        [Display(Name = "Correo")]
        public string Email { get; set; }

        public List<RoleModel> Roles { get; set; }

        [Display(Name = "Habilitado")]
        public bool IsEnabled { get; set; }
        public Guid RolId { get; set; }
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
            _accessor.HttpContext.Session.SetInt32("timeoutSessionInMilliseconds", 30000); //TODO:FDDFDF
            _accessor.HttpContext.Session.SetInt32("timeWaitingForAnswerInactivitySeconds", 30000); //TODO:FDDFDF
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
        public int UserId { get; set; }
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
using Ferreteria.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ferreteria.Domain.ViewModel
{
    #region account
    /// <summary>
    /// Modelo para la autenticacion
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Contraseña
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
    /// <summary>
    /// Modelo del usaurio
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// id del usuario
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Correo electronico
        /// </summary>
        public string Email { get; set; }
       
        /// <summary>
        /// Indica si se encientra habiltado o se deshabilitado el usuario
        /// </summary>
        public bool IsEnabled { get; set; }
    }
    /// <summary>
    /// Informacion del usuario con roles y privilegios
    /// </summary>
    public class UserModelComplete : UserModel
    {
        /// <summary>
        /// Roles del usaurio
        /// </summary>
        public List<ViewModel.Login.RoleModel> Roles { get; set; }

    }
    /// <summary>
    /// Modelo del usaurio
    /// </summary>
    public class UserSimpleModel
    {
        /// <summary>
        /// id del usuario
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Nombre de la cuenta del usuario
        /// </summary>
        public string UserName { get; set; }
    }
    /// <summary>
    /// Modelo para la paginacion de usuarios
    /// </summary>
    public class UserModelPaginator
    {
        /// <summary>
        /// Numero total de elementos
        /// </summary>
        public int TotalElements { get; set; }
        /// <summary>
        /// Numero de elementos por pagina
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Numero de la pagina
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Valor que se desea buscar
        /// </summary>
        public string SearchString { get; set; }
        /// <summary>
        /// Orden de la columna
        /// </summary>
        public string sortOrder { get; set; }
        public List<UserModel> List { get; set; }
    }
    /// <summary>
    /// Modelo de regitro de un usaurio
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        [Required]
        public string Username { get; set; }
        /// <summary>
        /// Correo elctronico
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }
        /// <summary>
        /// Contraseña
        /// </summary>
        [DataType(DataType.Password)]
        public string Password { get; set; }
        /// <summary>
        /// Nombre
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Telefeno
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Guid de el usuario en el directorio activo
        /// </summary>
        public Guid? LdapGuid { get; set; }

        /// <summary>
        /// Código de la entidad a la que esta asociado el usuario
        /// </summary>
        public string Organization { get; set; }
        /// <summary>
        /// Indica si se habilta o se da de baja el usuario
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Lista id de los roles que se le asignan al usuario
        /// </summary>
        public List<Guid> Roles { get; set; }
    }
    /// <summary>
    /// Token del usaurio
    /// </summary>
    public class TokenModel
    {
        /// <summary>
        /// Token
        /// </summary>
        [Required]
        public string UserToken { get; set; }
    }
    /// <summary>
    /// Modelo para cambiar la contraseña
    /// </summary>
    public class ChangePasswordModel
    {
        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Contaaseña actual
        /// </summary>
        public string OldPassword { get; set; }
        /// <summary>
        /// Nueva contraseña que se desea asignar
        /// </summary>
        public string NewPassword { get; set; }
    }
    /// <summary>
    /// Modelo para actualizar los datos del usaurio
    /// </summary>
    public class UserUpdateRequest
    {
        /// <summary>
        /// Id del usuario
        /// </summary>
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// Correo elctronico
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Nombre
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Telefeno
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Guid de el usuario en el directorio activo
        /// </summary>
        public Guid? LdapGuid { get; set; }
        /// <summary>
        /// Codigo de la entidad (No es relacional)
        /// </summary>
        public string Organization { get; set; }
        /// <summary>
        /// Usuario habilitado
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Lista id de los roles que se le asignan al usuario
        /// </summary>
        public List<Guid> Roles { get; set; }
        /// <summary>
        /// Indica si aceptó el Habeas Data
        /// </summary>
        public bool AcceptedHabeasData { get; set; }
        /// <summary>
        /// Indica si se aceptó los terminos y condiciones
        /// </summary>
        public bool AcceptedTermsAndConditions { get; set; }
    }

    /// <summary>
    /// Model del usaurio LDAP
    /// </summary>
    public class UserLdapRequest
    {
        /// <summary>
        /// Nombre de usaurio en el directorio activo
        /// </summary>
        public string UserName { get; set; }
    }
    /// <summary>
    /// Respuesta de los datos del usuario
    /// </summary>
    public class UserLdapResponse
    {
        /// <summary>
        /// Guid del usuario en el  directorio activo
        /// </summary>
        public Guid? ldapID { get; set; }
        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Correo electronico
        /// </summary>
        public string Email { get; set; }
    }
   
    /// <summary>
    /// Modelo para bloquear un usuario
    /// </summary>
    public class UserLockRequest
    {
        /// <summary>
        /// Id del usuario
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Indica si se bloquea o desbloque el usuario
        /// </summary>
        public bool Lock { get; set; }
    }
    /// <summary>
    /// Modelo para habilitar un usuario
    /// </summary>
    public class UserEnabledRequest
    {
        /// <summary>
        /// Id del usuario
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Indica si se habilta o se da de baja el usuario
        /// </summary>
        public bool Enabled { get; set; }
    }
    /// <summary>
    /// Modelo prara eviar el token de recuperacion
    /// </summary>
    public class RecoverPasswordResponse
    {
        /// <summary>
        /// token para validar el cambio de la contraseña
        /// </summary>
        [Required]
        public string token { get; set; }
    }
    /// <summary>
    /// Modelo para cambiar la contraseña ´por token
    /// </summary>
    public class RecoverPasswordResquest
    {
        /// <summary>
        /// Nombre de la cuenta del usuario
        /// </summary>
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// Contraseña
        /// </summary>
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// token para validar el cambio de la contraseña
        /// </summary>
        [Required]
        public string token { get; set; }
    }

    /// <summary>
    /// Modelo para cambiar la contraseña API
    /// </summary>
    public class PasswordChangeResquest
    {
        /// <summary>
        /// Nombre de la cuenta del usuario
        /// </summary>
        [Required]
        public string UserName { get; set; }
        /// <summary>
        /// Contraseña Actual
        /// </summary>
        [Required]
        public string OldPassword { get; set; }
        /// <summary>
        /// Nueva Contraseña
        /// </summary>
        [Required]
        public string NewPassword { get; set; }
    }

    /// <summary>
    /// Modelo para actualizar los datos basicos del usuario API
    /// </summary>
    public class UpdateInfoUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
    #endregion
}

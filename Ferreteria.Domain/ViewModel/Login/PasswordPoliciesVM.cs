using System;
using System.ComponentModel.DataAnnotations;

namespace Ferreteria.Domain.ViewModel.Login
{
    /// <summary>
    /// 
    /// </summary>
    public class PasswordPoliciesVM
    {
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "La contraseña debe contener al menos la siguiente cantidad de caracteres")]
        public string PasswordRequiredLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "La contraseña establecida tendrá una vigencia en días de")]
        public string PasswordExpiredDays { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Requiere un símbolo no alfanumérico")]
        public string PasswordRequireNonLetterOrDigit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Requiere Dígito")]
        public string PasswordRequireDigit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Requiere Letra Minúscula ")]
        public string PasswordRequireLowercase { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Requiere Letra Mayúscula ")]
        public string PasswordRequireUppercase { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PasswordPolciciesRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ApiToken { get; set; }
    }
}
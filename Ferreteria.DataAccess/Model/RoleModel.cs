using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ferreteria.DataAccess.Model
{
    #region role
    /// <summary>
    /// Información del Rol
    /// </summary>
    public class RoleModel
    {
        /// <summary>
        /// Id del Rol
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Nombre del rol
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Indica si el rol se encuentra habilitado o deshabilitado
        /// </summary>
        public bool IsEnabled { get; set; }
    }

    /// <summary>
    /// Datos para crear el Rol
    /// </summary>
    public class RoleModelRequest
    {
        /// <summary>
        /// Descripcion del rol
        /// </summary>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// Nombre del rol
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Lista de id de los privilegios que se le asignan al rol
        /// </summary>
        public List<Guid> Privileges { get; set; }
    }
    /// <summary>
    /// Datos del rol
    /// </summary>
    public class RoleModelRespose
    {
        /// <summary>
        /// Id del rol
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Nombre de la aplicacion
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// Descripcion
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Nombre
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Indica si el rol se encuentra habilitado o deshabilitado
        /// </summary>
        public bool Enabled { get; set; }
    }
    /// <summary>
    /// datos para asociar roles y privilegios
    /// </summary>
    public class RolePrivilegeRequest
    {
        /// <summary>
        /// Id del rol de la aplicacion
        /// </summary>
        [Required]
        public Guid RolId { get; set; }
        /// <summary>
        /// Id del privilegio de la aplicacion
        /// </summary>
        [Required]
        public Guid PrivId { get; set; }
    }
    /// <summary>
    /// Modelo para añadir un rol a un usuario
    /// </summary>
    public class AddRoleUserModel
    {
        /// <summary>
        /// Id del usuario
        /// </summary>
        [Required]
        public Guid UserId { get; set; }
        /// <summary>
        /// Id del rol de la aplicacion
        /// </summary>
        [Required]
        public Guid RolId { get; set; }
    }
    /// <summary>
    /// Datos para habilitar  o deshabilitar un rol
    /// </summary>
    public class RoleEnabledRequest
    {
        /// <summary>
        /// Id del rol de la aplicacion
        /// </summary>
        [Required]
        public Guid Id { get; set; }
        /// <summary>
        /// Indica si se habilita o deshabilita el rol.
        /// </summary>
        public bool Enabled { get; set; }
    }
    #endregion
}
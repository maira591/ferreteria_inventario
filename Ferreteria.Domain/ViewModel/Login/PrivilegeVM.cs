using System;

namespace Ferreteria.Domain.ViewModel.Login
{
    #region privilege
    /// <summary>
    /// Datos del privilegio
    /// </summary>
    public class PrivilegeModel
    {
        /// <summary>
        /// Id del privilegio
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nombre del privilegio
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Codigo del privilegio
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Indica si se encuentra habilitado o deshabilitado el privilegio
        /// </summary>
        public bool Enabled { get; set; }
    }
    /// <summary>
    /// DAtos pra crear un privilegio
    /// </summary>
    public class PrivilegeModelRequest
    {
        /// <summary>
        /// Nombre del privilegio
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Codigo del privilegio
        /// </summary>
        public string Code { get; set; }
    }
    /// <summary>
    /// Datos para habilitar un privilegio
    /// </summary>
    public class PrivilegeEnabledRequest
    {
        /// <summary>
        /// Id del privilegio
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Indica si se habilita o deshabilita el privilegio
        /// </summary>
        public bool Enabled { get; set; }
    }
    #endregion
}

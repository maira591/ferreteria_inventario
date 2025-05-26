using System;

namespace Ferreteria.Domain.ViewModel.Notification
{
    /// <summary>
    /// Solicitud para notificar los estados del proceso
    /// </summary>
    public class GeneralRequestVM
    {
        /// <summary>
        /// Código de la entidad solidaria o financiera o solidaria
        /// </summary>
        public string CodigoEntidad { get; set; }

        /// <summary>
        /// Fecha de corte
        /// </summary>
        public DateTime FechaCorte { get; set; }

        /// <summary>
        /// Este campo hace referencia al ID del estado (ID del Valor catálogo)
        /// </summary>
        public string Estado { get; set; }
        /// <summary>
        /// Nombre del archivo .zip que se está procesando.
        /// </summary>
        public string Detalles { get; set; }
    }
}
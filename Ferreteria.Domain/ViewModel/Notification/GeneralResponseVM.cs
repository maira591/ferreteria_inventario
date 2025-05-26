namespace Ferreteria.Domain.ViewModel.Notification
{
    /// <summary>
    /// Respuesta al consumo del servicio de notificación de estados del proceso
    /// </summary>
    public class GeneralResponseVM
    {
        /// <summary>
        /// Mensaje de respuesta de consumo del servicio
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Código de respuesta de ejecución del servicio
        /// </summary>
        public int Code { get; set; }
    }
}
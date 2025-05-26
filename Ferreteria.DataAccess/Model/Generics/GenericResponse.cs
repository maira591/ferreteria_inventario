using Ferreteria.Infrastructure.Extensions;
using System;
using System.Net;

namespace Ferreteria.DataAccess.Model.Generics
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericResponse<T> where T : class
    {
        /// <summary>
        /// Codigo de estado Http
        /// </summary>
        public HttpStatusCode Status { get; set; }
        /// <summary>
        /// Cuerpo
        /// </summary>
        public T Body { get; set; }
        /// <summary>
        /// Mensaje
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Se obtiene una respuesta sin errores
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static GenericResponse<T> GetSuccesResponse(int code, string message, T body)
        {
            GenericResponse<T> response = new GenericResponse<T>();
            response.Status = (HttpStatusCode)code;
            response.Body = body;
            response.Message = message;
            return response;
        }

        /// <summary>
        /// Se obtiene una respuesta con la traza del error
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static GenericResponse<T> GetErrorResponse(int code, Exception ex)
        {
            GenericResponse<T> response = new GenericResponse<T>();
            response.Status = (HttpStatusCode)code;
            response.Body = null;
            response.Message = ex.Message();
            return response;
        }
    }
}
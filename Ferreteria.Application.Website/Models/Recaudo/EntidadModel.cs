using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.Website.Models.Recaudo
{
    public class EntidadModel
    {
        [Display(Name = "Tipo Entidad")]
        public int TipoEntidad { get; set; }
        public int PreviousTipoEntidad { get; set; }
        public string NombreTipoEntidad { get; set; }
        [Display(Name = "Código Entidad")]
        public string CodigoEntidad { get; set; }
        public string PreviousCodigoEntidad { get; set; }
        [Display(Name = "Tipo Identificación")]
        public string TipoIdentificacion { get; set; }
        [Display(Name = "Número Identificación")]
        public string NumeroIdentificacion { get; set; }
        [Display(Name = "Digito Chequeo")]
        public string DigitoChequeo { get; set; }
        public int Clase { get; set; }
        [Display(Name = "Palabra Clave")]
        public string PalabraClave { get; set; }
        public string Grupo { get; set; }
        public string NombreGrupo { get; set; }
        public string SubGrupo { get; set; }
        [Display(Name = "Tipo Estado")]
        public int TipoEstado { get; set; }
        public string NombreTipoEstado { get; set; }
        public int Estado { get; set; }
        public string NombreEstado { get; set; }
        [Display(Name = "Fecha Constitución")]
        public DateTime FechaConstitucion { get; set; }
        [Display(Name = "Fecha Autorización")]
        public DateTime FechaAutorizacion { get; set; }
        [Display(Name = "Fecha Inscripción")]
        public DateTime FechaInscripcion { get; set; }
        [Display(Name = "Fecha Final Inscripción")]
        public DateTime? FechaFinalInscripcion { get; set; }
        [Display(Name = "Tipo Entidad SES")]
        public int TipoEntidadSes { get; set; }
        public string NombreTipoEntidadSes { get; set; }
        [Display(Name = "Estado Entidad SES")]
        public string EstadoEntidadSes { get; set; }
        public string NombreEstadoSes { get; set; }
        [Display(Name = "Perfil Asociados")]
        public string PerfilAsociados { get; set; }
        public int? IsNew { get; set; }
    }
}

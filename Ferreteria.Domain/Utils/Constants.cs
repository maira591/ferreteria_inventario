using DocumentFormat.OpenXml;
using System.ComponentModel.DataAnnotations;

namespace Ferreteria.Domain.Utils
{
    public static class Constants
    {
        public enum LoadDataErrorTypes
        {
            FormatStructure = 1,
            DataValidation = 2,
            VerticalValidation = 3
        }

        public enum PeriodicitySIG
        {
            Mensual,
            Trimestral,
            Semestral,
            Anual
        }

        public static class StylesIndexExcel
        {
            /// <summary>
            /// Tipo letra: Arial, Color: Negro, Tamaño: 11, Negrilla: No, Color de fondo: Blanco
            /// </summary>
            public static UInt32Value EstiloPorDefecto = 0U;

            /// <summary>
            /// Tipo letra: Arial, Color: Azul, Tamaño: 12, Color de fondo: Gris, Negrilla: Si
            /// </summary>
            public static UInt32Value EstiloEncabezados = 1U;

            /// <summary>
            /// Fuente 3 - Tipo letra: Arial, Color: Blanco, Tamaño: 12, Negrilla: Si, Color de fondo: Verde,
            /// </summary>
            public static UInt32Value EstiloEncabezadosPlantillaFormato = 2U;

            /// <summary>
            /// Tipo letra: Arial, Color: Negro, Tamaño: 11, Color de fondo: Gris
            /// </summary>
            public static UInt32Value EstiloColumnasPorDefectoPlantillaFormato = 3U;
        }

        public const string KeySchemaRecaudo = "recaudoSchema";

        public const string DefaultDecimalSeparator = ".";

        public const string EntityCodeParameter = "CodigoEntidad";
        public const string FolderDefault = "Correspondencia\\Cooperativa\\";

        public enum UseUnitRow
        {
            [Display(Name = "Renglón unidad de captura")]
            RenglonUnidadDeCaptura = 1,
            [Display(Name = "Renglón")]
            Renglon = 2,
            [Display(Name = "Unidad de captura")]
            UnidadDeCaptura = 3
        }
    }

    public class ConstantsValueCatalogs
    {
        public const string InicioProcesoZip = "InicioProcesoZip";
        public const string DescargaZip = "DescargaZip";
        public const string DescomprimirZip = "DescomprimirZip";
        public const string RecordPerSend = "RecordPerSend";
        public const string InicioInsertarInformacionFormatos = "InicioInsertarInformacionFormatos";
        public const string FinInsertarInformacionFormatos = "FinInsertarInformacionFormatos";
        public const string InicioValidaciones = "InicioValidaciones";
        public const string ErrorValidacionFormato = "ErrorValidacionFormato";
        public const string ErrorProcesamientoZip = "ErrorProcesamientoZip";
        public const string FinProcesamientoZip = "FinProcesamientoZip";
        public const string ValidacionFormatos = "ValidacionFormatos";
        public const string InicioProcesarFormato = "InicioProcesarFormatos";
        public const string FinProcesarFormato = "FinProcesarFormatos";
        public const string ProcesamientoZip = "ProcesamientoZip";
        public const string InsertarInformacionFormatos = "InsertarInformacionFormatos";
        public const string ErrorInsertarInformacionFormatos = "ErrorInsertarInformacionFormatos";
        public const string FinValidaciones = "FinValidaciones";
        public const string ExisteInformacionProcesada = "ExisteInformacionProcesada";
        public const string SeparadorDecimal = "SeparadorDecimal";
        public const string PasarInformacionMonitoreo = "PasarInformacionMonitoreo";
        public const string InicioPasoInformacionMonitoreo = "InicioPasoInformacionMonitoreo";
        public const string FinPasoInformacionMonitoreo = "FinPasoInformacionMonitoreo";
        public const string ErrorPasoInformacionMonitoreo = "ErrorPasoInformacionMonitoreo";
        public const string EmailNotificationValueCatalogCode = "SendEmailMethod";
        public const string CantidadDias = "CantidadDias";
        public const string CantidadMeses = "CantidadMeses";
        public const string MaximoMesesCorte = "MaximoMesesCorte";
        public const string NombreAplicacion = "NombreAplicacion";
        public const string CorreosNotificarCargaArchivosSIG = "CorreosNotificarCargaArchivosSIG";
        public const string RutaBaseDictorioArchivos = "RutaBaseDictorioArchivos";
        public const string PlantillaNotificacionCorreo = "PlantillaNotificacionCorreo";
        public const string PesoLimiteCargaArchivoSIG = "PesoLimiteCargaArchivoSIG";
        public const string DiaCalculoAutomatico = "DiaCalculoAutomatico";
        public const string NumeroDiasFechaCorteLiquidacion = "NumeroDiasFechaCorteLiquidacion";
        public const string HabilitarCalculoAutomaticoLiquidacion = "HabilitarCalculoAutomaticoLiquidacion";

    }

    public class ConstantsCatalogs
    {
        public const string EstadosCarga = "EstadosCarga";
        public const string ProcesosCarga = "ProcesosCarga";
        public const string EmailNotificationCatalogCode = "EmailNotification";
        public const string ReportPermissions = "ReportPermissions";
        public const string FormuladorPalabrasReservadas = "FormuladorPalabrasReservadas";
        public const string TablasReporteSIDCORE = "TablasReporteSIDCORE";
        public const string TablasReporteEsquemaExterno = "TablasReporteEsquemaExterno";
        public const string EntidadAplicaFormato = "EntidadAplicaFormato";
        public const string NombresTasasMonitoreo = "NombresTasasMonitoreo";
        public const string BetasMonitoreo = "BetasMonitoreo";
        public const string FechaMaximaPagoRecaudo = "FechaMaximaPagoRecaudo";
        public const string GruposEntidadRecaudo = "RecaudoEntidadGrupos";
        public const string ExtensionesFormatos = "ExtensionesFormatos";
        public const string GrupoIndicadores = "GruposIndicadoresGraficas";
        public const string PeriodicidadGraficas = "PeriodicidadGraficas";
        public const string CodigosAgregados = "CodigosAgregados";
        public const string ExtensionesArchivosSIG = "ExtensionesArchivosSIG";
        public const string ConfiguracionCargaArchivosSIG = "ConfiguracionCargaArchivosSIG";
        public const string ConfiguracionCalculoAutomatico = "ConfiguracionCalculoAutomatico";
    }
    public class FormatReadingConfigurationEnum
    {
        public const string CaptureUnit = "UnidadCaptura";
        public const string Renglon = "Renglon";
        public const string Fila = "Fila";
        public const string DefaultValueCaptureUnit = "1";
        public const string PositionCaptureUnitInTheTxtFile = "2";
        public const string PositionRenglonInTheTxtFile = "3";
        public const int PositionCaptureUnitInTheExcelFileFinancial = 1;
        public const int PositionRenglonInTheExcelFileFinancial = 2;
        public const int PositionCaptureUnitInTheExcelFileSolidarie = 0;
        public const int PositionRenglonInTheExcelFileSolidarie = 1;
        public const string NumberOfExtraColumns = "3";
        public const int ErrorTypeFormatStructure = 1;
        public const int ErrorTypeValidationData = 2;
        public const int ErrorTypeVerticalValidation = 3;
        public const string TypeValidationCorrective = "Correctiva";
        public const string NameCooperativeTypeFinancial = "financiera";
        public const string NameCooperativeTypeSolidarie = "solidaria";
        public const string AliasFormatSolidarie = "balancesreportados";
        public const string AliasFormatFinancial = "balances";
        public const int PossitionRowWhereTheCutoffCateIs = 2;
        public const int NumberRowWhereTheCutoffCateIs = 3;
        public const int RowWhereDefaultReadingStarts = 2;
        public const int ColumnWhereTheReadingStartsByDefault = 4;
        public const string ExtensionCSV = "csv";
        public const string ExtensionXLSX = "xlsx";
        public const string ExtensionXLS = "xls";
        public const string ExtensionTXT = "txt";
    }

    public class ConstantsLoadInformation
    {
        public const int Rechazado = 0;
        public const int Aceptado = 1;
        public const int EnProceso = 2;
    }

    public class ConstantsMasterTable
    {
        public const int IsCreate = 0;
        public const int IsUpdate = 1;
    }

    public class ConstantsRestransmissionMonitoreo
    {
        public const int NotFinished = 0;
    }

    public class ConstantsMonitoreo
    {
        public const string ConstantRangeTypes = "RANGO";
        public const string ConstantClasification = "CLA";
        public const int MinYearFecIniCapRate = 2000;
    }

    public class ConstantsRecaudo
    {
        public const string EntidadTipoIdentificacion = "NIT";
        public const int EntidadTipoEstadoSes = 5;
        public const int EntidadClase = 2;
        public const string EntidadPalabraClave = "X";
    }
    public enum NumberPart
    {
        Integer = 0,
        Decimal = 1
    }

    public class ConstantsSecurity
    {
        public const string user = "UserBasicAuth";
        public const string pass = "PasswordBasicAuth";

    }

    public class ListType
    {
        public const string Simple = "Selección Simple";
        public const string Multiple = "Selección Múltiple";

        public const int TypeSimple = 1;
        public const int TypeMultiple = 2;

    }

    public class TypeFormatMonitoreo
    {
        public const string Solidary = "Solidaria";
        public const string Financial = "Financiera";
        public const string FinancialAndSolidarity = "Solidaria y Financiera";

        public const int TypeSolidary = 1;
        public const int TypeFinancial = 2;
        public const int TypeFinancialAndSolidarity = 3;
    }

    public class PeriodicityType
    {
        public const string Semanal = "semanal";
        public const string Quincenal = "quincenal";
        public const string Esporadica = "esporadica";

        public const int TreintaDias = 30;
        public const double PromedioDiasMeses = 30.44;
        public const int DoceMeses = 12;
    }

    public static class GraphicPosition
    {
        public const string Top = "top";
        public const string Bottom = "bottom";
        public const string Left = "left";
        public const string Right = "right";

    }

    public static class GraphicFill
    {
        public const string Origin = "origin";
        public const string Line = "line";
        public const string False = "false";
    }

    public static class GraphicType
    {
        public const string Radar = "radar";
        public const string CajaBigotes = "boxplot";
        public const string Line = "line";
        public const string Bar = "bar";
    }

    public static class GraphicLabelField
    {
        public const string NameGraphicLabelField = "fechacorte";
    }

    public static class GraphicIndicatorTypeField
    {
        public const string Line = "line";
        public const string Bar = "bar";
        public const string Area = "area";
    }

    public static class GraphicTypeName
    {
        public const string Linea = "Linea";
        public const string CajasBigotes = "Bigotes";
        public const string BarrasLinea = "Barras/Linea";
        public const string Barras = "Barras";
        public const string Radar = "Radar";
    }

    public static class GraphicCode
    {
        public const string Radar = "Radar";
        public const string Indicator = "Indicator";
        public const string GrossPortfolioLineGraphic = "GrossPortfolioLineGraphic";
        public const string DepositsLineGraphic = "DepositsLineGraphic";
    }


    public static class TyeUser
    {
        public const string Cooperative = "1";
        public const string Fogacoop = "2";
    }
}

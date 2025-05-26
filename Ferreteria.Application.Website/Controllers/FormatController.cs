using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.FormatColumnService;
using Core.Domain.FormatLineService;
using Core.Domain.FormatService;
using Core.Domain.OfficeService;
using Core.Domain.PeriodicityService;
using Core.Domain.ViewModel;
using Core.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class FormatController : Controller
    {
        #region Private fields
        private readonly IFormatService _formatService;
        private readonly string _apitoken;
        private readonly IMapper _mapper;
        private readonly IPeriodicityService _periodicityService;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IFormatLineService _formatLineService;
        private readonly IFormatColumnService _formatColumnService;
        private readonly IOfficeService _officeService;
        #endregion

        #region Constructor
        public FormatController(IFormatService formatService, IConfigurator configurationService, IMapper mapper, IUserBasicModel userBasicModel,
             IPeriodicityService periodicityService, IFormatLineService formatLineService, IOfficeService officeService,
             IFormatColumnService formatColumnService)
        {
            _formatService = formatService;
            var cfgService = configurationService;
            _apitoken = cfgService.GetKey("ApiToken");
            _mapper = mapper;
            _periodicityService = periodicityService;
            _userBasicModel = userBasicModel;
            _formatLineService = formatLineService;
            _officeService = officeService;
            _formatColumnService = formatColumnService;
        }
        #endregion

        #region Public Methods
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var model = _mapper.Map<List<FormatVM>, List<FormatModel>>((await _formatService.GetAllAsync()).ToList());
            model.ForEach(e => e.PeriodicitiesNames = (e.FormatPeriodicityList.Count > 0) ?
                                string.Join(",", e.FormatPeriodicityList.Select(x => x.Periodicity.Name)) : "");
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear formato";
            ViewBag.ListPeriodicityTypes = await PeriodicityList();
            ViewBag.ListFormatTypes = await FormatTypeList();
            return PartialView("_CreateOrEdit", new FormatModel { IsEnabled = true });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            ViewBag.TitleModal = "Editar formato";
            var format = await _formatService.GetByIdAsync(new Guid(id));
            ViewBag.ListFormatTypes = await FormatTypeList();
            ViewBag.ListPeriodicityTypes = await PeriodicityList();
            FormatModel model = _mapper.Map<FormatModel>(format);
            model.GuidFormatTypeId = format.FormatTypeId.ToString();
            return PartialView("_CreateOrEdit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(FormatModel format)
        {
            try
            {
                format.AppId = new Guid(_apitoken);
                if (format.Id == Guid.Empty)
                {
                    format.CreatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    format.CreatedOn = DateTime.Now;
                    format.IsNew = true;
                }
                else
                {
                    format.UpdatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    format.UpdatedOn = DateTime.Now;
                }
                format.FormatTypeId = new Guid(format.GuidFormatTypeId);
                await _formatService.AddOrUpdateAsync(_mapper.Map<FormatVM>(format));

                return Json(JsonResponseFactory.SuccessResponse("Formato guardado correctamente."));
            }
            catch (Exception e)
            {
                string err = e.Message;

                if (err.Contains("ORA-00001"))
                {
                    return Json(JsonResponseFactory.ErrorResponse("El nombre ingresado ya existe y está siendo utilizado por otro formato."));
                }
                else
                {
                    return Json(JsonResponseFactory.ErrorResponse($"Ocurrió un error inesperado: {err}"));
                }
            }
        }


        [HttpGet]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _formatService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó el formato correctamente."));
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("||") >= 0)
                {
                    return Json(JsonResponseFactory.ErrorResponse(ex.Message.Replace("||", "")));
                }
                return Json(JsonResponseFactory.ErrorResponse("No fue posible eliminar el registro."));
            }
        }

        [HttpGet]
        public async Task<FileResult> ExportTemplateFormat(Guid id)
        {
            FormatVM format = (await _formatService.GetAllAsync()).Where(x => x.Id == id).FirstOrDefault();
            List<FormatLineExcelVM> lstLines = _mapper.Map<List<FormatLineVM>, List<FormatLineExcelVM>>(await _formatLineService.GetByFormatId(format.Id))
                                  .OrderBy(x => x.CodeCaptureUnit).ThenBy(x => x.Code).ToList();
            return File(_officeService.GenerateTemplateFormat(format, lstLines), $"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Plantilla - {format.Name}.xlsx");
        }

        [HttpPost]
        public async Task<ActionResult> ExportData(string fileName)
        {
            var sheetsData = new List<List<object>>();
            var sheetsName = new List<string>();

            var formats = (await _formatService.GetAllAsync()).ToList();
            var columns = (await _formatColumnService.GetAllAsync()).ToList();
            var formatLines = (await _formatLineService.GetAllAsync()).ToList();

            var newFormats = formats.Select(x => new
            {
                TipoFormato = x.Type.Name,
                Codigo = x.Code,
                Nombre = x.Name,
                Alias = x.Alias,
                Periodicidad = string.Join(",", x.FormatPeriodicityList.Select(x => x.Periodicity.Name)),
                Descripcion = x.Description,
                TablaAlmacenamiento = x.StorageTable,
                Obligatorio = (x.IsRequired ? "SI" : "NO"),
                Activo = (x.IsEnabled ? "SI" : "NO"),
                FormatoTraspuesto = (x.IsTrasposed ? "SI" : "NO")
            }).ToList<object>();

            var newColumns = columns.Select(x => new
            {
                Formato = x.Format.Name,
                x.NombreColumna,
                x.TipoDato,
                Longitud = x.LongitudDato,
                AceptaNulos = (x.AceptaNulos ? "SI" : "NO"),
                NombreAMostrar = x.NombreColumnaExcel,
                x.Orden,
                Homologacion = (x.Homologation != null ? x.Homologation.Name : ""),
                ColumnaAlmacenamiento = x.StorageColumn,
                Calculado = x.IsCalculated != 0 ? "SI" : "NO",
                Formula = x.Formula
            }).OrderBy(x => x.Formato).ToList<object>();

            var newformatLines = formatLines.Select(x => new
            {
                Formato = x.Format.Name,
                UnidadCaptura = x.CaptureUnit.Code,
                CodigoRenglon = x.Code,
                DescripcionRenglon = x.Description
            }).OrderBy(x => x.Formato).ToList<object>();

            sheetsData.Add(newFormats);
            sheetsName.Add("Formatos");
            sheetsData.Add(newColumns);
            sheetsName.Add("Columnas");
            sheetsData.Add(newformatLines);
            sheetsName.Add("Renglones");

            return File(_officeService.GenerateExcelFromTheParameterizedData(sheetsData, sheetsName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(FormatModel format)
        {
            string message = string.Empty;

            format.FormatTypeId = new Guid(format.GuidFormatTypeId);

            message = await _formatService.Validations(_mapper.Map<FormatVM>(format));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }
        #endregion

        #region Private Methods
        private async Task<List<SelectListItem>> FormatTypeList()
        {
            List<Domain.ViewModel.ValueCatalogVM> formatTypes = await _formatService.GetFormatTypes();
            var formatTypeList = Common.LoadList(formatTypes, "Name", "Id");
            if (formatTypeList == null)
            {
                return new List<SelectListItem>();
            }
            return formatTypeList;

        }

        private async Task<List<SelectListItem>> PeriodicityList()
        {
            List<Domain.ViewModel.PeriodicityVM> periodicityTypes = (await _periodicityService.GetAllAsync()).Where(x => x.IsEnabled).ToList();
            var list = Common.LoadList(periodicityTypes, "Name", "Id");
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }
        #endregion

    }
}
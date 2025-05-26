using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Models.Monitoreo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CatalogService;
using Core.Domain.Monitoreo.MatInfoAdicionalService;
using Core.Domain.Monitoreo.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class MonitoreoMatInfoAdicionalController : Controller
    {
        #region Private fields

        private readonly IMatInfoAdicionalService _matInfoAdicionalService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly ICatalogService _catalogService;

        #endregion

        #region Constructor
        public MonitoreoMatInfoAdicionalController(IMatInfoAdicionalService matInfoAdicionalService, IMapper mapper, IUserBasicModel userBasicModel, ICatalogService catalogService)
        {
            _matInfoAdicionalService = matInfoAdicionalService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _catalogService = catalogService;
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
            var model = _mapper.Map<List<MatInfoAdicionalVM>, List<MatInfoAdicionalModel>>((await _matInfoAdicionalService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<JsonResult> GetListEntityCodeByTypeEntity(int typeEntity)
        {
            var lstSigla = await _matInfoAdicionalService.GetListEntityCodeByTypeEntity(typeEntity);

            return Json(lstSigla);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Indicador";
            ViewBag.ListTypeEntity = await GetListTypeEntity();
            ViewBag.ListIndicators = await GetListIndicators();
            ViewBag.ListEntityCodeByTypeEntity = await GetListEntityCode(0);
            return PartialView("_CreateOrEdit", new MatInfoAdicionalModel());
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            try
            {
                ViewBag.TitleModal = "Editar Indicador";

                ViewBag.ListTypeEntity = await GetListTypeEntity();
                ViewBag.ListIndicators = await GetListIndicators();
                var formato = await _matInfoAdicionalService.GetByIdAsync(int.Parse(id));
                MatInfoAdicionalModel model = _mapper.Map<MatInfoAdicionalModel>(formato);
                model.Valor = model.Valor.Replace(",", ".");
                ViewBag.ListEntityCodeByTypeEntity = await GetListEntityCode(model.TipoEntidad);
                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(MatInfoAdicionalModel model)
        {
            try
            {
                model.Valor = model.Valor.Replace(",", ".");
                model.FechaRegistro = DateTime.Now;
                await _matInfoAdicionalService.AddOrUpdateAsync(_mapper.Map<MatInfoAdicionalVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Indicador guardado correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(MatInfoAdicionalModel model)
        {
            string message = string.Empty;

            message = await _matInfoAdicionalService.Validations(_mapper.Map<MatInfoAdicionalVM>(model));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }
        #endregion

        #region Private Methods

        private async Task<List<SelectListItem>> GetListTypeEntity()
        {
            var lstTypeEntities = await _matInfoAdicionalService.GetListTypeEntity();
            var lstObject = lstTypeEntities.Select(x => new { Name = x.Descripcion, Id = x.IdTipoEntidad }).ToList();

            var selectList = Common.LoadList(lstObject, "Name", "Id");

            if (selectList == null)
            {
                return new List<SelectListItem>();
            }

            return selectList;
        }

        private async Task<List<SelectListItem>> GetListIndicators()
        {
            var lstIndicators = await _matInfoAdicionalService.GetListIndicators();
            var lstObject = lstIndicators.Select(x => new { Name = x.Nombre, x.Id }).ToList();

            var selectList = Common.LoadList(lstObject, "Name", "Id");

            if (selectList == null)
            {
                return new List<SelectListItem>();
            }

            return selectList;
        }

        private async Task<List<SelectListItem>> GetListEntityCode(int typeEntity)
        {
            if (typeEntity == 0)
            {
                return new List<SelectListItem>();
            }

            var lstIndicators = await _matInfoAdicionalService.GetListEntityCodeByTypeEntity(typeEntity);

            var lstObject = lstIndicators.Select(x => new { Name = $"{x.CodigoEntidad} - {x.Sigla}", Id = x.CodigoEntidad }).ToList();

            var selectList = Common.LoadList(lstObject, "Name", "Id");

            if (selectList == null)
            {
                return new List<SelectListItem>();
            }

            return selectList;
        }



        #endregion
    }
}
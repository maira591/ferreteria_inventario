using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CatalogService;
using Core.Domain.OfficeService;
using Core.Domain.ValuesCatalogService;
using Core.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class CatalogController : Controller
    {
        #region Private fields
        private readonly ICatalogService _catalogService;
        private readonly IValueCatalogService _valueCatalogService;
        private readonly string _apitoken;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IOfficeService _officeService;
        #endregion

        public CatalogController(ICatalogService catalogService, IValueCatalogService valueCatalogService, IConfigurator configurationService, IMapper mapper, IUserBasicModel userBasicModel, IOfficeService officeService)
        {
            _catalogService = catalogService;
            _valueCatalogService = valueCatalogService;
            var cfgService = configurationService;
            _apitoken = cfgService.GetKey("ApiToken");
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _officeService = officeService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var model = _mapper.Map<List<Domain.ViewModel.CatalogVM>, List<CatalogModel>>((await _catalogService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            ViewBag.TitleModal = "Crear catálogo";
            return PartialView("_CreateOrEdit");
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            ViewBag.TitleModal = "Editar catálogo";
            var catalog = await _catalogService.GetByIdAsync(new Guid(id));

            return PartialView("_CreateOrEdit", _mapper.Map<CatalogModel>(catalog));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(CatalogModel catalog)
        {
            try
            {
                catalog.AppId = new Guid(_apitoken);
                if (catalog.Id == Guid.Empty)
                {
                    catalog.CreatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    catalog.CreatedOn = DateTime.Now;
                }
                else
                {
                    catalog.UpdatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    catalog.UpdatedOn = DateTime.Now;
                }
                await _catalogService.AddOrUpdateAsync(_mapper.Map<Domain.ViewModel.CatalogVM>(catalog));

                return Json(JsonResponseFactory.SuccessResponse("Catálogo guardado correctamente."));
            }
            catch (Exception e)
            {
                var err = e.Message;
                if (err.Contains("ORA-00001"))
                {
                    return Json(JsonResponseFactory.ErrorResponse("El código ingresado ya existe y está siendo utilizado por otro catálogo."));
                }
                else
                {
                    return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _catalogService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó el catálogo correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("No fue posible elimiar el registro."));
            }
        }

        [HttpPost]
        public async Task<ActionResult> ExportCatalogs(string fileName)
        {
            var sheetsData = new List<List<object>>();
            var sheetsName = new List<string>();
            var catalogs = (await _catalogService.GetAllAsync()).ToList();
            var valueCatalogs = (await _valueCatalogService.GetAllAsync()).ToList();
            var newCatalogs = (from catalogo in catalogs select new { Codigo = catalogo.Code, Nombre = catalogo.Name, Descripcion = catalogo.Description }).OrderBy(t => t.Codigo).ToList<object>();
            var newValueCatalogs = (from valueCatalog in valueCatalogs select new { Catalogo = valueCatalog.Catalog.Code, Codigo = valueCatalog.Code, Nombre = valueCatalog.Name, Descripcion = valueCatalog.Description, Cifrado = valueCatalog.IsEncrypted ? "Sí" : "No" }).OrderBy(t => t.Catalogo).ToList<object>();
            sheetsData.Add(newCatalogs);
            sheetsName.Add("Catálogos");
            sheetsData.Add(newValueCatalogs);
            sheetsName.Add("Valores Catálogo");

            return File(_officeService.GenerateExcelFromTheParameterizedData(sheetsData, sheetsName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }
    }
}
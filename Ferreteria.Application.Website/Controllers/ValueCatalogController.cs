using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.ValuesCatalogService;
using Core.Domain.ViewModel;
using Core.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fogacoop.Application.Website.Areas.Management.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class ValueCatalogController : Controller
    {
        #region Private fields
        private readonly IValueCatalogService _valueCatalogService;
        private readonly string _apitoken;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IMapper mapperValueCatalog;
        private readonly IUserBasicModel _userBasicModel;
        #endregion

        public ValueCatalogController(IValueCatalogService valueCatalogService, IConfigurator configurationService, IMapper mapper, IUserBasicModel userBasicModel)
        {
            _valueCatalogService = valueCatalogService;
            var cfgService = configurationService;
            _apitoken = cfgService.GetKey("ApiToken");
            mapperValueCatalog = mapper;
            _userBasicModel = userBasicModel;
        }

        [HttpGet]
        public async Task<PartialViewResult> Index(Guid catalogId)
        {
            ViewBag.TitleModal = "Crear Valor Catálogo";

            return PartialView("_IndexGrid", mapperValueCatalog.Map<List<ValuesCatalogModel>>(await _valueCatalogService.GetAllByCatalogIdAsync(catalogId)));
        }

        [HttpGet]
        public PartialViewResult Create(Guid catalogId)
        {
            ViewBag.TitleModal = "Crear Valor Catálogo";

            return PartialView("_CreateOrEdit", new ValuesCatalogModel() { CatalogId = catalogId });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(Guid id)
        {
            ViewBag.TitleModal = "Editar Valor Catálogo";
            var valueCatalog = await _valueCatalogService.GetByIdAsync(id);

            return PartialView("_CreateOrEdit", mapperValueCatalog.Map<ValuesCatalogModel>(valueCatalog));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(ValuesCatalogModel valueCatalog)
        {
            try
            {
                //valueCatalog.Id = valueCatalog.Id == Guid.Empty ? Guid.NewGuid() : valueCatalog.Id;
                if (valueCatalog.Id == Guid.Empty)
                {
                    valueCatalog.CreatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    valueCatalog.CreatedOn = DateTime.Now;
                }
                else
                {
                    valueCatalog.UpdatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    valueCatalog.UpdatedOn = DateTime.Now;
                }

                await _valueCatalogService.AddOrUpdateAsync(mapperValueCatalog.Map<ValueCatalogVM>(valueCatalog));

                return Json(JsonResponseFactory.SuccessResponse("Valor catálogo guardado correctamente."));
            }
            catch (Exception e)
            {
                var err = e.Message;
                if (err.Contains("ORA-00001"))
                {
                    return Json(JsonResponseFactory.ErrorResponse("El código ingresado ya existe y está siendo utilizado por otro valor catálogo."));
                }
                else
                {
                    return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(Guid id, Guid idCatalog)
        {
            try
            {
                await _valueCatalogService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó el valor correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("No fue posible elimiar el registro."));
            }
        }
    }
}
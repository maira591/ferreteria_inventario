using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.HomologationService;
using Core.Domain.HomologationValueService;
using Core.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fogacoop.Application.Website.Areas.Management.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class HomologationValueController : Controller
    {
        #region Private fields
        private readonly IHomologationValueService _homologationValueService;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IHomologationService _homologationService;

        #endregion

        public HomologationValueController(IHomologationValueService homologationValueService, IMapper mapper, IUserBasicModel userBasicModel, IHomologationService homologationService)
        {
            _homologationValueService = homologationValueService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _homologationService = homologationService;
        }

        [HttpGet]
        public async Task<PartialViewResult> Index(Guid homologationId)
        {
            return PartialView("_IndexGrid", _mapper.Map<List<HomologationValueModel>>(await _homologationValueService.GetAllByHomologationIdAsync(homologationId)));
        }

        [HttpGet]
        public async Task<PartialViewResult> Create(Guid homologationId)
        {
            ViewBag.TitleModal = "Crear valor homologación";
            var homologation = await _homologationService.GetByIdAsync(homologationId);

            HomologationValueModel model = new();
            model.HomologationId = homologationId;
            model.HomologationName = homologation.Name;

            return PartialView("_CreateOrEdit", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(Guid id)
        {
            ViewBag.TitleModal = "Editar valor homologación";
            var homologationValue = await _homologationValueService.GetByIdAsync(id);

            return PartialView("_CreateOrEdit", _mapper.Map<HomologationValueModel>(homologationValue));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(HomologationValueModel homologationValue)
        {
            try
            {
                if (homologationValue.Id == Guid.Empty)
                {
                    homologationValue.CreatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    homologationValue.CreatedOn = DateTime.Now;
                    homologationValue.IsNew = true;
                }
                else
                {
                    homologationValue.UpdatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                    homologationValue.UpdatedOn = DateTime.Now;
                }

                await _homologationValueService.AddOrUpdateAsync(_mapper.Map<HomologationValueVM>(homologationValue));

                return Json(JsonResponseFactory.SuccessResponse("Valor homologación guardado correctamente."));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Concat("Error en HomologationValueController - CreateOrUpdate: ", ex.Message));
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _homologationValueService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó el valor correctamente."));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Concat("Error en HomologationValueController - Delete: ", ex.Message));
                return Json(JsonResponseFactory.ErrorResponse("No fue posible eliminar el registro."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(HomologationValueModel homologationValue)
        {
            string message = string.Empty;

            message = await _homologationValueService.Validations(_mapper.Map<HomologationValueVM>(homologationValue));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }
    }
}
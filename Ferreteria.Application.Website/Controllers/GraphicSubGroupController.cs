using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.GraphicGroupService;
using Core.Domain.GraphicSubGroupService;
using Core.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fogacoop.Application.Website.Areas.Management.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class GraphicSubGroupController : Controller
    {
        #region Private fields

        private readonly IGraphicSubGroupService _graphicSubGroupService;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IGraphicGroupService _graphicGroupService;

        #endregion

        public GraphicSubGroupController(IGraphicSubGroupService graphicSubGroupService, IMapper mapper, IUserBasicModel userBasicModel, IGraphicGroupService graphicGroupService)
        {
            _graphicSubGroupService = graphicSubGroupService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _graphicGroupService = graphicGroupService;
        }

        [HttpGet]
        public async Task<PartialViewResult> Index(Guid graphicGroupId)
        {
            return PartialView("_IndexGrid", _mapper.Map<List<GraphicSubGroupModel>>(await _graphicSubGroupService.GetAllByGraphicGroupIdAsync(graphicGroupId)));
        }

        [HttpGet]
        public async Task<PartialViewResult> Create(Guid graphicGroupId)
        {
            ViewBag.TitleModal = "Crear sub grupo gráfico";

            var graphicGroupVM = await _graphicGroupService.GetByIdAsync(graphicGroupId);

            GraphicSubGroupModel model = new();
            model.GraphicGroupId = graphicGroupId;
            model.GraphicGroupName = graphicGroupVM.Name;

            return PartialView("_CreateOrEdit", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(Guid id)
        {
            ViewBag.TitleModal = "Editar sub grupo gráfico";
            var graphicSubGroup = await _graphicSubGroupService.GetByIdAsync(id);
            var model = _mapper.Map<GraphicSubGroupModel>(graphicSubGroup);

            return PartialView("_CreateOrEdit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(GraphicSubGroupModel graphicSubGroup)
        {
            try
            {
                if (graphicSubGroup.Id == Guid.Empty)
                {
                    graphicSubGroup.CreatedBy = _userBasicModel.GetCurrentUser().UserName;
                    graphicSubGroup.CreatedOn = DateTime.Now;
                    graphicSubGroup.IsNew = true;
                }
                else
                {
                    graphicSubGroup.UpdatedBy = _userBasicModel.GetCurrentUser().UserName;
                    graphicSubGroup.UpdatedOn = DateTime.Now;
                }

                await _graphicSubGroupService.AddOrUpdateAsync(_mapper.Map<GraphicSubGroupVM>(graphicSubGroup));

                return Json(JsonResponseFactory.SuccessResponse("Sub grupo guardado correctamente."));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Concat("Error en GraphicSubGroupController - CreateOrUpdate: ", ex.Message));
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _graphicSubGroupService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó el registro correctamente."));
            }
            catch (Exception ex)
            {
                _logger.Error(string.Concat("Error en GraphicSubGroupController - Delete: ", ex.Message));
                return Json(JsonResponseFactory.ErrorResponse("No fue posible eliminar el registro."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(GraphicSubGroupModel graphicSubGroup)
        {
            string message = string.Empty;

            message = await _graphicSubGroupService.Validations(_mapper.Map<GraphicSubGroupVM>(graphicSubGroup));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }
    }
}
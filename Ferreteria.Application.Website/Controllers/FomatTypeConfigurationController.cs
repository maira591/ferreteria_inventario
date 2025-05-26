using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CatalogService;
using Core.Domain.FomatTypeConfigurationService;
using Core.Domain.FormatService;
using Core.Domain.Utils;
using Core.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class FomatTypeConfigurationController : Controller
    {
        #region Private fields
        private readonly IFomatTypeConfigurationService _fomatTypeConfigurationService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IFormatService _formatService;
        private readonly ICatalogService _catalogService;

        #endregion

        public FomatTypeConfigurationController(IFomatTypeConfigurationService fomatTypeConfigurationService, IMapper mapper, IUserBasicModel userBasicModel,
                                 IFormatService formatService, ICatalogService catalogService)
        {
            _fomatTypeConfigurationService = fomatTypeConfigurationService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _formatService = formatService;
            _catalogService = catalogService;
        }

        [HttpGet]
        public async Task<PartialViewResult> Index(Guid formatId)
        {
            var model = _mapper.Map<List<FomatTypeConfigurationVM>, List<FomatTypeConfigurationModel>>((await _fomatTypeConfigurationService.GetAllAsyncByFormatId(formatId)).ToList());
            ViewBag.ExtensionsFormatList = await GetExtensionsFormats();
            ViewBag.FormatId = formatId;

            ViewBag.TitleModal = "Configuración de extensión del formato: " + await GetFormatName(formatId);

            return PartialView("Index", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveFormatLine(List<FomatTypeConfigurationModel> listFormatLine)
        {
            try
            {
                foreach (FomatTypeConfigurationModel formatLine in listFormatLine)
                {
                    if (formatLine.Id == Guid.Empty)
                    {
                        formatLine.CreatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                        formatLine.CreatedOn = DateTime.Now;
                        formatLine.IsNew = true;
                    }
                    else
                    {
                        formatLine.UpdatedBy = _userBasicModel.GetCurrentUser().Id.ToString();
                        formatLine.UpdatedOn = DateTime.Now;
                    }
                }

                await _fomatTypeConfigurationService.AddOrUpdateAsync(_mapper.Map<List<FomatTypeConfigurationModel>, List<FomatTypeConfigurationVM>>(listFormatLine));

                return Json(JsonResponseFactory.SuccessResponse("Configuración de lectura del formato guardado correctamente."));
            }
            catch (Exception e)
            {
                var err = e.Message;

                if (err.Contains("ORA-00001"))
                {
                    return Json(JsonResponseFactory.ErrorResponse("El nombre ingresado ya existe y está siendo utilizado por otro formato."));
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
                await _fomatTypeConfigurationService.DeleteAsync(id);
                return Json(JsonResponseFactory.SuccessResponse("Se eliminó el registro correctamente."));
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

        private async Task<List<SelectListItem>> GetExtensionsFormats()
        {
            List<ValueCatalogVM> listValueCatalogs = (await _catalogService.GetByCodeAsync(ConstantsCatalogs.ExtensionesFormatos)).ValueCatalogs;

            var list = Common.LoadList(listValueCatalogs, "Name", "Id");
            if (list == null)
            {
                return new List<SelectListItem>();
            }
            return list;
        }

        private async Task<string> GetFormatName(Guid formatId)
        {
            FormatVM format = await _formatService.GetByIdAsync(formatId);
            return format.Name;
        }

    }
}
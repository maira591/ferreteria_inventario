using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CaptureUnitService;
using Core.Domain.FormatLineService;
using Core.Domain.FormatService;
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
    public class FormatLineController : Controller
    {
        #region Private fields
        private readonly IFormatLineService _formatLineService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly ICaptureUnitService _captureUnitService;
        private readonly IFormatService _formatService;

        #endregion

        public FormatLineController(IFormatLineService formatLineService, IMapper mapper, IUserBasicModel userBasicModel,
                                    ICaptureUnitService captureUnitService, IFormatService formatService)
        {
            _formatLineService = formatLineService;
            _mapper = mapper;
            _captureUnitService = captureUnitService;
            _userBasicModel = userBasicModel;
            _formatService = formatService;
        }

        [HttpGet]
        public async Task<PartialViewResult> Index(Guid formatId)
        {
            var model = _mapper.Map<List<FormatLineVM>, List<FormatLineModel>>((await _formatLineService.GetAllAsyncByFormatId(formatId)).ToList());
            ViewBag.CaptureUnitList = await GetCaptureUnitList();
            ViewBag.FormatId = formatId;

            ViewBag.TitleModal = "Renglones Formato: " + await GetFormatName(formatId);

            return PartialView("Index", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveFormatLine(List<FormatLineModel> listFormatLine)
        {
            try
            {
                foreach (FormatLineModel formatLine in listFormatLine)
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

                await _formatLineService.AddOrUpdateAsync(_mapper.Map<List<FormatLineModel>, List<FormatLineVM>>(listFormatLine));

                return Json(JsonResponseFactory.SuccessResponse("Renglones formato guardados correctamente."));
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
                await _formatLineService.DeleteAsync(id);
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

        private async Task<List<SelectListItem>> GetCaptureUnitList()
        {
            List<CaptureUnitVM> listCaptureUnit = (await _captureUnitService.GetAllAsync()).Where(x => x.IsEnabled).ToList();
            listCaptureUnit.ForEach(x => x.Name = string.Concat(x.Code, "-", x.Name));
            var captureUnitList = Common.LoadList(listCaptureUnit, "Name", "Id");
            if (captureUnitList == null)
            {
                return new List<SelectListItem>();
            }
            return captureUnitList;
        }

        private async Task<string> GetFormatName(Guid formatId)
        {
            FormatVM format = await _formatService.GetByIdAsync(formatId);
            return format.Name;
        }

    }
}
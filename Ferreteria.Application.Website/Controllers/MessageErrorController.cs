using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.MessageErrorService;
using Core.Domain.OfficeService;
using Core.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class MessageErrorController : Controller
    {
        #region Private fields
        private readonly IMessageErrorService _messageErrorService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly IOfficeService _officeService;
        #endregion

        #region Constructor

        public MessageErrorController(IMessageErrorService messageErrorService, IMapper mapper, IUserBasicModel userBasicModel, IOfficeService officeService)
        {
            _messageErrorService = messageErrorService;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
            _officeService = officeService;
        }

        #endregion

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            return PartialView("_IndexGrid", await _messageErrorService.GetAllAsync());
        }

        [HttpGet]
        public PartialViewResult Create()
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            ViewBag.TitlePage = "Crear mensaje";
            MessageVM message = new();
            return PartialView("_CreateOrEdit", message);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            ViewBag.UrlBase = Common.GetUrlBase(Request);
            var messageError = await _messageErrorService.GetByIdAsync(new Guid(id));
            ViewBag.TitlePage = "Editar mensaje";
            return PartialView("_CreateOrEdit", messageError);
        }

        [HttpPost]
        public async Task<JsonResult> SaveInfo(MessageVM messageError)
        {
            var user = _userBasicModel.GetCurrentUser();

            try
            {
                var exists = _messageErrorService.GetByCode(messageError.Code);
                if (exists != null)
                {
                    if (messageError.Id != exists.Id)
                    {
                        return Json(JsonResponseFactory.ErrorResponse("El código del mensaje de error ya existe."));
                    }
                }

                if (messageError.Id == Guid.Empty)
                {
                    messageError.CreatedBy = user.Id.ToString();
                    messageError.CreatedOn = DateTime.Now;
                }
                else
                {
                    messageError.UpdatedBy = user.Id.ToString();
                    messageError.UpdatedOn = DateTime.Now;
                }
                await _messageErrorService.AddOrUpdateAsync(messageError);

                return Json(JsonResponseFactory.SuccessResponse("Mensaje de error guardado correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpPost]
        public async Task<ActionResult> ExportMessageErrors(string fileName)
        {
            var sheetsData = new List<List<object>>();
            var sheetsName = new List<string>();

            var messageErrors = (await _messageErrorService.GetAllAsync()).ToList();

            var newMessageErrors = (from message in messageErrors select new { Codigo = message.Code, Nombre = message.Name, Mensaje = message.Description, PosibleSolucion = message.PossibleSolution, ErrorNoControlado = message.Exception ? "Si" : "No", ErrorDelBus = message.Bus ? "Si" : "No" }).ToList<object>();
            sheetsData.Add(newMessageErrors);
            sheetsName.Add("Mensajes de error");

            return File(_officeService.GenerateExcelFromTheParameterizedData(sheetsData, sheetsName), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}");
        }
    }
}
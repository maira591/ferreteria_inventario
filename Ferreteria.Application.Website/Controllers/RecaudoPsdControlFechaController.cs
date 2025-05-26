using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Models.Recaudo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CatalogService;
using Core.Domain.Recaudo.PsdControlFechaService;
using Core.Domain.Recaudo.ViewModel;
using Core.Domain.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class RecaudoPsdControlFechaController : Controller
    {

        #region Private fields

        private readonly IPsdControlFechaService _psdControlFechaService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly ICatalogService _catalogService;

        #endregion

        #region Constructor
        public RecaudoPsdControlFechaController(IPsdControlFechaService psdControlFechaService, IMapper mapper, IUserBasicModel userBasicModel, ICatalogService catalogService)
        {
            _psdControlFechaService = psdControlFechaService;
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
            var model = _mapper.Map<List<PsdControlFechaVM>, List<PsdControlFechaModel>>((await _psdControlFechaService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Fecha De control";
            ViewBag.NumberMonths = GetNumberMonths();
            var model = await _psdControlFechaService.GetLastRecord();
            model.IsNew = ConstantsMasterTable.IsCreate;
            return PartialView("_CreateOrEdit", _mapper.Map<PsdControlFechaModel>(model));
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string fechaCorte)
        {
            try
            {
                ViewBag.TitleModal = "Editar Fecha De control";
                ViewBag.NumberMonths = GetNumberMonths();
                var formato = await _psdControlFechaService.GetByCutoffDate(Utilities.DateParse(fechaCorte).Value);
                PsdControlFechaModel model = _mapper.Map<PsdControlFechaModel>(formato);

                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetParamsByCutoffDate(PsdControlFechaModel model)
        {
            try
            {
                return Json(await _psdControlFechaService.GetParamsByCutoffDate(_mapper.Map<PsdControlFechaVM>(model)));
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(PsdControlFechaModel model)
        {
            string message = string.Empty;

            message = await _psdControlFechaService.Validations(_mapper.Map<PsdControlFechaVM>(model));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(PsdControlFechaModel model)
        {
            try
            {
                await _psdControlFechaService.AddOrUpdateAsync(_mapper.Map<PsdControlFechaVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Fecha de control guardada correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }
        #endregion


        #region Private Methods
        private List<SelectListItem> GetNumberMonths()
        {
            List<SelectListItem> selectList = new();

            for (int i = 1; i < 13; i++)
            {
                selectList.Add(new() { Value = $"{i}", Text = $"{i}" });
            }

            return selectList;
        }
        #endregion
    }
}
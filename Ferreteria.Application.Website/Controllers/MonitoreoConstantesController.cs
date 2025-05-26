using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Models.Monitoreo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.Monitoreo.ConstantesService;
using Core.Domain.Monitoreo.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class MonitoreoConstantesController : Controller
    {
        #region Private fields

        private readonly IConstantesService _constantesservice;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;

        #endregion

        public MonitoreoConstantesController(IConstantesService constantesservice, IMapper mapper, IUserBasicModel userBasicModel)
        {
            _constantesservice = constantesservice;
            _mapper = mapper;
            _userBasicModel = userBasicModel;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var model = _mapper.Map<List<ConstantesVM>, List<ConstantesModel>>((await _constantesservice.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear constante";
            ViewBag.ParentList = await GetParentsList();
            return PartialView("_CreateOrEdit", new ConstantesModel { Estado = true });
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            try
            {
                ViewBag.TitleModal = "Editar constante";

                ViewBag.ParentList = await GetParentsList();
                var constante = await _constantesservice.GetByIdAsync(int.Parse(id));
                ConstantesModel model = _mapper.Map<ConstantesModel>(constante);

                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(ConstantesModel model)
        {
            try
            {

                await _constantesservice.AddOrUpdateAsync(_mapper.Map<ConstantesVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Constante guardada correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(ConstantesModel model)
        {
            string message = string.Empty;

            message = await _constantesservice.Validations(_mapper.Map<ConstantesVM>(model));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }

        private async Task<List<SelectListItem>> GetParentsList()
        {
            List<ConstantesVM> parentList = await _constantesservice.GetParents();
            var selectList = Common.LoadList(parentList, "Nombre", "Id");
            if (selectList == null)
            {
                return new List<SelectListItem>();
            }
            return selectList;

        }
    }
}
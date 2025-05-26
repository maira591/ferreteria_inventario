using AutoMapper;
using Core.Application.Website.Models.Recaudo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.Recaudo.DepartamentosService;
using Core.Domain.Recaudo.ViewModel;
using Core.Domain.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class RecaudoDepartamentosController : Controller
    {
        #region Private Fields
        private readonly IDepartamentosService _departamentosService;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public RecaudoDepartamentosController(IDepartamentosService departamentosService, IMapper mapper)
        {
            _departamentosService = departamentosService;
            _mapper = mapper;
        }
        #endregion

        #region Public Methods
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var model = _mapper.Map<List<DepartamentosVM>, List<DepartamentosModel>>((await _departamentosService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Departamento";
            var model = new DepartamentosModel() { IsNew = ConstantsMasterTable.IsCreate };
            return PartialView("_CreateOrEdit", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(int id)
        {
            try
            {
                ViewBag.TitleModal = "Editar Departamento";

                var tasa = await _departamentosService.GetByIdAsync(id);
                DepartamentosModel model = _mapper.Map<DepartamentosModel>(tasa);

                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(DepartamentosModel model)
        {
            try
            {
                await _departamentosService.AddOrUpdateAsync(_mapper.Map<DepartamentosVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Departamento guardado correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(DepartamentosModel model)
        {
            string message = await _departamentosService.Validations(_mapper.Map<DepartamentosVM>(model));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }
        #endregion
    }
}

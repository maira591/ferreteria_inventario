using AutoMapper;
using Core.Application.Website.Models.Recaudo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.Recaudo.DepartamentosService;
using Core.Domain.Recaudo.MunicipiosService;
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
    public class RecaudoMunicipiosController : Controller
    {
        #region Private Fields
        private readonly IMunicipiosService _municipiosService;
        private readonly IMapper _mapper;
        private readonly IDepartamentosService _departamentosService;
        #endregion

        #region Constructor
        public RecaudoMunicipiosController(IMunicipiosService municipiosService, IMapper mapper, IDepartamentosService departamentosService)
        {
            _municipiosService = municipiosService;
            _mapper = mapper;
            _departamentosService = departamentosService;
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
            var model = _mapper.Map<List<MunicipiosVM>, List<MunicipiosModel>>((await _municipiosService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Municipio";
            ViewBag.ListDepartaments = await DepartmentsList();
            var model = new MunicipiosModel() { IsNew = ConstantsMasterTable.IsCreate };
            return PartialView("_CreateOrEdit", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(string id)
        {
            try
            {
                ViewBag.TitleModal = "Editar Municipio";

                var municipio = await _municipiosService.GetByIdAsync(id);
                ViewBag.ListDepartaments = await DepartmentsList();
                var model = _mapper.Map<MunicipiosModel>(municipio);

                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(MunicipiosModel model)
        {
            try
            {
                await _municipiosService.AddOrUpdateAsync(_mapper.Map<MunicipiosVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Municipio guardado correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(MunicipiosModel model)
        {
            string message = await _municipiosService.Validations(_mapper.Map<MunicipiosVM>(model));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }


        [HttpPost]
        public async Task<ActionResult> CalculateCodMunicipio(string departamento, string municipio)
        {
            var depart = string.IsNullOrEmpty(departamento) ? 0 : int.Parse(departamento);
            var munici = string.IsNullOrEmpty(municipio) ? 0 : int.Parse(municipio);
            var result = await _municipiosService.CalculateCodMunicipio(depart, munici);
            return Json(result);
        }
        #endregion


        #region Private Methods 
        public async Task<List<SelectListItem>> DepartmentsList(int id = 0)
        {
            List<DepartamentosVM> betas = (await _departamentosService.GetAllAsync()).OrderBy(x => x.Descripcion).ToList();
            if (!id.Equals(0))
            {
                betas.RemoveAll(t => t.Departamento != id);
            }

            var list = Common.LoadList(betas, "Descripcion", "Departamento");
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }
        #endregion 

    }
}

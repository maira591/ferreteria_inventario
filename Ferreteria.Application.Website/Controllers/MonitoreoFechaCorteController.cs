using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Models.Monitoreo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CatalogService;
using Core.Domain.Monitoreo.MonFechaCorteService;
using Core.Domain.Monitoreo.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class MonitoreoFechaCorteController : Controller
    {

        #region Private fields
        private readonly IMonFechaCorteService _monFechaCorteService;
        private readonly IMapper _mapper;
        private readonly IUserBasicModel _userBasicModel;
        private readonly ICatalogService _catalogService;
        #endregion

        #region Constructor
        public MonitoreoFechaCorteController(IMonFechaCorteService monFechaCorteService, IMapper mapper, IUserBasicModel userBasicModel, ICatalogService catalogService)
        {
            _monFechaCorteService = monFechaCorteService;
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
            var model = _mapper.Map<List<MonFechaCorteVM>, List<MonFechaCorteModel>>((await _monFechaCorteService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(int id)
        {
            try
            {
                ViewBag.TitleModal = "Editar Fecha Lanzamiento Flujo";

                var fechaCorte = await _monFechaCorteService.GetByIdAsync(id);
                MonFechaCorteModel model = _mapper.Map<MonFechaCorteModel>(fechaCorte);

                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(MonFechaCorteModel model)
        {
            try
            {
                await _monFechaCorteService.AddOrUpdateAsync(_mapper.Map<MonFechaCorteVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Fecha Lanzamiento Flujo guardada correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }
        #endregion

    }
}
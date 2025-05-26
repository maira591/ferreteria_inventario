using AutoMapper;
using Core.Application.Website.Models.Recaudo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.Recaudo.EntidadesService;
using Core.Domain.Recaudo.EstadoService;
using Core.Domain.Recaudo.TipoEntidadService;
using Core.Domain.Recaudo.TipoEntidadSesService;
using Core.Domain.Recaudo.TipoEstadoService;
using Core.Domain.Recaudo.ViewModel;
using Core.Domain.Utils;
using Core.Domain.ValuesCatalogService;
using Core.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.AdminCore)]
    public class RecaudoEntidadController : Controller
    {

        #region Private Fields
        private readonly IEntidadService _entidadesService;
        private readonly IMapper _mapper;
        private readonly ITipoEntidadService _tipoEntidadService;
        private readonly IValueCatalogService _valueCatalogService;
        private readonly ITipoEstadoService _tipoEstadoService;
        private readonly IEstadosService _estadosService;
        private readonly ITipoEntidadSesService _tipoEntidadSesService;
        #endregion

        #region Constructor
        public RecaudoEntidadController(IEntidadService entidadesService, IMapper mapper, ITipoEntidadService tipoEntidadService, IValueCatalogService valueCatalogService, ITipoEstadoService tipoEstadoService, IEstadosService estadosService, ITipoEntidadSesService tipoEntidadSesService)
        {
            _entidadesService = entidadesService;
            _mapper = mapper;
            _tipoEntidadService = tipoEntidadService;
            _valueCatalogService = valueCatalogService;
            _tipoEstadoService = tipoEstadoService;
            _estadosService = estadosService;
            _tipoEntidadSesService = tipoEntidadSesService;
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
            var model = _mapper.Map<List<EntidadVM>, List<EntidadModel>>((await _entidadesService.GetAllAsync()).ToList());

            return PartialView(
                "_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Entidad";
            ViewBag.TipoEntidadList = await TipoEntidadList();
            ViewBag.GrupoList = await GrupoList();
            ViewBag.TipoEstadoList = await TipoEstadoList();
            ViewBag.EstadoList = new List<SelectListItem>();
            ViewBag.TipoEntidadSesList = await TipoEntidadSesList();
            ViewBag.EstadoEntidaSesList = await EstadoList(ConstantsRecaudo.EntidadTipoEstadoSes);
            var model = new EntidadModel() { IsNew = ConstantsMasterTable.IsCreate };
            return PartialView("_CreateOrEdit", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(int tipoEntidad, string codigoEntidad)
        {
            try
            {
                ViewBag.TitleModal = "Editar Entidad";

                var entidad = await _entidadesService.GetByTipoEntidadAndCodigoEntidadAsync(tipoEntidad, codigoEntidad);
                var model = _mapper.Map<EntidadModel>(entidad);

                ViewBag.TipoEntidadList = await TipoEntidadList(0, model.TipoEntidad.ToString());
                ViewBag.GrupoList = await GrupoList("", model.Grupo);
                ViewBag.TipoEstadoList = await TipoEstadoList(0, model.TipoEstado.ToString());
                ViewBag.EstadoList = await EstadoList(model.TipoEstado, model.Estado.ToString());
                ViewBag.TipoEntidadSesList = await TipoEntidadSesList(0, model.TipoEntidadSes.ToString());
                ViewBag.EstadoEntidaSesList = await EstadoList(ConstantsRecaudo.EntidadTipoEstadoSes, model.EstadoEntidadSes);

                model.PreviousCodigoEntidad = model.CodigoEntidad;
                model.PreviousTipoEntidad = model.TipoEntidad;

                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(EntidadModel model)
        {
            try
            {
                await _entidadesService.AddOrUpdateAsync(_mapper.Map<EntidadVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Entidad guardado correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(EntidadModel model)
        {
            string message = await _entidadesService.Validations(_mapper.Map<EntidadVM>(model));

            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }


        [HttpPost]
        public async Task<JsonResult> ObtenerEstados(int idTipoEstado)
        {
            var estados = await EstadoList(idTipoEstado);
            return Json(estados);
        }
        #endregion

        #region Private Methods
        private async Task<List<SelectListItem>> TipoEntidadList(int id = 0, string selectedValue = "")
        {
            List<TipoEntidadVM> tipoEntidades = (await _tipoEntidadService.GetAllAsync()).OrderBy(x => x.Descripcion).ToList();
            if (!id.Equals(0))
            {
                tipoEntidades.RemoveAll(t => t.IdTipoEntidad != id);
            }

            var list = Common.LoadList(tipoEntidades, "Descripcion", "IdTipoEntidad", selectedValue);
            if (list == null)
            {
                return new List<SelectListItem>();
            }
            return list;
        }

        private async Task<List<SelectListItem>> GrupoList(string code = "", string selectedValue = "")
        {
            List<ValueCatalogVM> grupoList = (await _valueCatalogService.GetValueCatalogsWithCatalogCodeAsync(ConstantsCatalogs.GruposEntidadRecaudo)).OrderBy(x => x.Name).ToList();

            if (code != string.Empty)
            {
                grupoList.RemoveAll(t => t.Code != code);
            }

            var list = Common.LoadList(grupoList, "Name", "Code", selectedValue);
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }

        private async Task<List<SelectListItem>> TipoEstadoList(int id = 0, string selectedValue = "")
        {
            List<TipoEstadoVM> tipoEstado = (await _tipoEstadoService.GetAllAsync()).OrderBy(x => x.Descripcion).ToList();
            if (!id.Equals(0))
            {
                tipoEstado.RemoveAll(t => t.IdTipoEstado != id);
            }

            var list = Common.LoadList(tipoEstado, "Descripcion", "IdTipoEstado", selectedValue);
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }

        private async Task<List<SelectListItem>> EstadoList(int idTipoEstado = 0, string selectedValue = "")
        {
            List<EstadosVM> tipoEstado = (await _estadosService.GetAllByTipoEstadoAsync(idTipoEstado)).OrderBy(x => x.Descripcion).ToList();
            //if (!idTipoEstado.Equals(0))
            //{
            //    tipoEstado.RemoveAll(t => t.Estado != idTipoEstado);
            //}

            var list = Common.LoadList(tipoEstado, "Descripcion", "Estado", selectedValue);
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }

        private async Task<List<SelectListItem>> TipoEntidadSesList(int idTipoEntidad = 0, string selectedValue = "")
        {
            List<TipoEntidadSesVM> tipoEntidadSes = (await _tipoEntidadSesService.GetAllAsync()).OrderBy(x => x.Descripcion).ToList();
            if (!idTipoEntidad.Equals(0))
            {
                tipoEntidadSes.RemoveAll(t => t.TipoEntidad != idTipoEntidad);
            }

            var list = Common.LoadList(tipoEntidadSes, "Descripcion", "TipoEntidad", selectedValue);
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }
        #endregion
    }
}

using AutoMapper;
using Core.Application.Website.Models;
using Core.Application.Website.Models.Recaudo;
using Core.Application.Website.Providers;
using Core.Application.Website.Utils;
using Core.Domain.CooperativeService;
using Core.Domain.Recaudo.DepartamentosService;
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
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Website.Controllers
{
    [Auth(PrivilegesEnum.UpdateOrganizationInformation)]
    public class RecaudoEntidadesController : Controller
    {

        #region Private Fields
        private readonly IUserBasicModel _userBasicModel;
        private readonly UserBasicModel _currentUser;
        private readonly IDepartamentosService _departamentosService;
        private readonly IEntidadesService _entidadesService;
        private readonly IMapper _mapper;
        private readonly ITipoEntidadService _tipoEntidadService;
        private readonly IValueCatalogService _valueCatalogService;
        private readonly ITipoEstadoService _tipoEstadoService;
        private readonly IEstadosService _estadosService;
        private readonly ITipoEntidadSesService _tipoEntidadSesService;
        #endregion

        #region Constructor
        public RecaudoEntidadesController(IServiceProvider _serviceProvider)
        {
            _userBasicModel = _serviceProvider.GetService<IUserBasicModel>();
            _currentUser = _userBasicModel.GetCurrentUser();
            _entidadesService = _serviceProvider.GetService<IEntidadesService>();
            _departamentosService = _serviceProvider.GetService<IDepartamentosService>();
            _mapper = _serviceProvider.GetService<IMapper>();
            _tipoEntidadService = _serviceProvider.GetService<ITipoEntidadService>();
            _valueCatalogService = _serviceProvider.GetService<IValueCatalogService>();
            _tipoEstadoService = _serviceProvider.GetService<ITipoEstadoService>();
            _estadosService = _serviceProvider.GetService<IEstadosService>();
            _tipoEntidadSesService = _serviceProvider.GetService<ITipoEntidadSesService>();
        }
        #endregion

        #region Public Methods
        public async Task<IActionResult> Index()
        {
            var model = new EntidadesModel();
            var listDepartment = new List<SelectListItem>();
            var listMunicipios = new List<SelectListItem>();
            var listSiglas = new List<SelectListItem>();
            if (!string.IsNullOrWhiteSpace(_currentUser.Organization))
            {
                var cooperative = await _entidadesService.GetByCodigoEntidadAsync(_currentUser.Organization);
                model = _mapper.Map<EntidadesModel>(cooperative);
                listDepartment = await DepartmentsList();
            }
            ViewBag.ListMunicipios = listMunicipios;
            ViewBag.ListSiglas = listSiglas;
            ViewBag.ListDepartaments = listDepartment;
            return View(model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Entidad";
            ViewBag.TipoEntidadList = await TipoEntidadList();
            ViewBag.GrupoList = await GrupoList();
            var model = new EntidadModel() { IsNew = ConstantsMasterTable.IsCreate };
            return PartialView("_CreateOrEdit", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(int tipoEntidad, string codigoEntidad)
        {
            try
            {
                ViewBag.TitleModal = "Editar Organización";

                var entidad = await _entidadesService.GetByTipoEntidadAndCodigoEntidadAsync(tipoEntidad, codigoEntidad);
                var model = _mapper.Map<EntidadesModel>(entidad);

                ViewBag.TipoEntidadList = await TipoEntidadList(0, model.TipoEntidad.ToString());

                return PartialView("_CreateOrEdit", model);
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrUpdate(EntidadesModel model)
        {
            try
            {
                await _entidadesService.AddOrUpdateAsync(_mapper.Map<EntidadesVM>(model));

                return Json(JsonResponseFactory.SuccessResponse("Entidad guardado correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(EntidadesModel model)
        {
            string message = await _entidadesService.Validations(_mapper.Map<EntidadesVM>(model));

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

        private async Task<List<SelectListItem>> DepartmentsList(int id = 0)
        {
            List<DepartamentosVM> departamentos = (await _departamentosService.GetAllAsync()).OrderBy(x => x.Descripcion).ToList();
            if (!id.Equals(0))
            {
                departamentos.RemoveAll(t => t.Departamento != id);
            }

            var list = Common.LoadList(departamentos, "Descripcion", "Departamento");
            if (list == null)
            {
                return new List<SelectListItem>();
            }

            return list;
        }
        #endregion
    }
}

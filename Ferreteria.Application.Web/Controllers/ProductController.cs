using AutoMapper;
using Ferreteria.Application.Website.Models;
using Ferreteria.Application.Website.Utils;
using Ferreteria.Domain.CategoryService;
using Ferreteria.Domain.ProductService;
using Ferreteria.Domain.SupplierService;
using Ferreteria.Domain.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core.Application.Website.Controllers
{
    public class ProductController(IProductService productService, IMapper mapper, ISupplierService supplierService, ICategoryService categoryService) : Controller
    {
        #region Public Methods
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> IndexGrid()
        {
            var model = mapper.Map<List<ProductVM>, List<ProductModel>>((await productService.GetAllAsync()).ToList());
            return PartialView("_IndexGrid", model);
        }

        [HttpGet]
        public async Task<PartialViewResult> Create()
        {
            ViewBag.TitleModal = "Crear Producto";
            ViewBag.ListSupplier = await SupplierList();
            ViewBag.ListCategory = await CategoryList();
            ViewBag.ListUnitType = UnitTypeList();

            return PartialView("_CreateOrEdit", new ProductModel());
        }

        [HttpGet]
        public async Task<PartialViewResult> Edit(int id)
        {
            ViewBag.TitleModal = "Editar Producto";

            ViewBag.ListSupplier = await SupplierList();
            ViewBag.ListCategory = await CategoryList();
            ViewBag.ListUnitType = UnitTypeList();

            var product = await productService.GetByIdAsync(id);

            ProductModel model = mapper.Map<ProductModel>(product);

            return PartialView("_CreateOrEdit", model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrUpdate(ProductModel product)
        {
            try
            {
                await productService.AddOrUpdateAsync(mapper.Map<ProductVM>(product));
                return Json(JsonResponseFactory.SuccessResponse("Producto guardado correctamente."));
            }
            catch
            {
                return Json(JsonResponseFactory.ErrorResponse("Ocurrió un error inesperado."));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Validations(ProductModel product)
        {
            string message = await productService.Validations(mapper.Map<ProductVM>(product));
            return new JsonResult(new { Message = message, Valid = string.IsNullOrWhiteSpace(message) });
        }
        #endregion

        #region Private Methods
        private async Task<List<SelectListItem>> SupplierList()
        {
            List<SupplierVM> users = (await supplierService.GetAllAsync()).ToList();
            var list = Common.LoadList(users, "Name", "SupplierId");

            if (list == null)
            {
                return [];
            }

            return list;
        }

        private async Task<List<SelectListItem>> CategoryList()
        {
            List<CategoryVM> users = (await categoryService.GetAllAsync()).ToList();
            var list = Common.LoadList(users, "Name", "CategoryId");

            if (list == null)
            {
                return [];
            }

            return list;
        }

        private static List<SelectListItem> UnitTypeList()
        {
            List<SelectListItem> returnList =
            [
                new SelectListItem { Text = "Unidad", Value = "Unidad" },
                new SelectListItem { Text = "Caja", Value = "Caja" },
                new SelectListItem { Text = "Metro", Value = "Metro" },
                new SelectListItem { Text = "Rollo", Value = "Rollo" },
                new SelectListItem { Text = "Litros", Value = "Litros" },
                new SelectListItem { Text = "Galones", Value = "Galones" },
                new SelectListItem { Text = "Kilogramos", Value = "Kilogramos" },
                new SelectListItem { Text = "Gramos", Value = "Gramos" },
                new SelectListItem { Text = "Pulgada", Value = "Pulgada" },
                new SelectListItem { Text = "Mililitros", Value = "Mililitros" },
                new SelectListItem { Text = "Tubo", Value = "Tubo" },
                new SelectListItem { Text = "Hoja", Value = "Hoja" },

                new SelectListItem { Text = "Paquete", Value = "Paquete" },
                new SelectListItem { Text = "Docena", Value = "Docena" },
                new SelectListItem { Text = "Blíster", Value = "Blíster" },
                new SelectListItem { Text = "Par", Value = "Par" },
                new SelectListItem { Text = "Juego", Value = "Juego" },
                new SelectListItem { Text = "Frasco", Value = "Frasco" },
                new SelectListItem { Text = "Bidón", Value = "Bidón" },
                new SelectListItem { Text = "Libras", Value = "Libras" },
                new SelectListItem { Text = "Onzas", Value = "Onzas" },
                new SelectListItem { Text = "Centímetro", Value = "Centímetro" },
                new SelectListItem { Text = "Pie", Value = "Pie" }
            ];

            return returnList;
        }


        #endregion
    }
}

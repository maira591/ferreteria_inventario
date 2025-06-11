using AutoMapper;
using Ferreteria.DataAccess.Core;
using Ferreteria.DataAccess.Model;
using Ferreteria.Domain.LogService;
using Ferreteria.Domain.ViewModel;
using Ferreteria.Infrastructure.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ferreteria.Domain.ProductService
{
    public class ProductService(IUnitOfWork unitOfWork, IMapper mapper, ILogService logService) : IProductService
    {
        #region Methods
        public async Task AddOrUpdateAsync(ProductVM product)
        {
            const string action = "Product AddOrUpdateAsync";

            try
            {
                logService.Add(LogKey.Begin, action);
                var productDataAccess = mapper.Map<Product>(product);

                productDataAccess.Category = null;
                productDataAccess.Supplier = null;
                if (product.ProductId != default)
                {
                    unitOfWork.Repository<Product>().Update(productDataAccess);
                }
                unitOfWork.Repository<Product>().AddOrUpdate(productDataAccess);

                logService.Add(LogKey.Request, JsonConvert.SerializeObject(productDataAccess));
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var err = e.Message();
                logService.Add(LogKey.Error, $"Trace: {e.Trace()}, Error Message: {err}");
                throw new Exception(err);
            }
            finally
            {
                logService.Add(LogKey.End, action);
                logService.Generate();
            }
        }

        public async Task<IEnumerable<ProductVM>> GetAllAsync()
        {
            try
            {
                var productDataAccess = (await unitOfWork.Repository<Product>().GetAsync("Category", "Supplier")).ToList();

                var products = mapper.Map<List<Product>, List<ProductVM>>(productDataAccess);
                return products;
            }
            catch (Exception ex)
            {
                string err = ex.Message();
                logService.Add(LogKey.Error, $"Trace: {ex.Trace()}, Error Message: {err}");
                throw new Exception(err);
            }
            finally
            {
                logService.Generate();
            }
        }

        public async Task<ProductVM> GetByIdAsync(int id)
        {
            var productDataAccess = await unitOfWork.Repository<Product>().FindAsync(id);

            var product = mapper.Map<ProductVM>(productDataAccess);
            return product;
        }

        public async Task<string> Validations(ProductVM product)
        {
            string validation = string.Empty;

            if ((await unitOfWork.Repository<Product>().GetAsync(x => x.ProductId != product.ProductId && x.CategoryId == product.CategoryId && x.Name.ToUpper() == product.Name.Trim().ToUpper())).Any())
            {
                validation = $"El nombre '{product.Name}' del producto ya existe categoria seleccionada.";
            }

            return validation;
        }
        #endregion
    }
}
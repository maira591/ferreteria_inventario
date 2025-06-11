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

namespace Ferreteria.Domain.CategoryService
{
    public class CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogService logService) : ICategoryService
    {
        #region Methods
        public async Task AddOrUpdateAsync(CategoryVM category)
        {
            const string action = "Category AddOrUpdateAsync";

            try
            {
                logService.Add(LogKey.Begin, action);
                var categoryDataAccess = mapper.Map<Category>(category);

                if (category.CategoryId != default)
                {
                    unitOfWork.Repository<Category>().Update(categoryDataAccess);
                }
                unitOfWork.Repository<Category>().AddOrUpdate(categoryDataAccess);

                logService.Add(LogKey.Request, JsonConvert.SerializeObject(categoryDataAccess));
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


        public async Task DeleteAsync(int id)
        {
            bool isUsedPermit = (await unitOfWork.Repository<Product>().GetAsync(x => x.CategoryId == id)).Any();

            if (!isUsedPermit)
            {
                unitOfWork.Repository<Category>().Remove(t => t.CategoryId.Equals(id));
                await unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new Exception("El proveedor está asociado a uno o más productos, no se puede eliminar.||");
            }
        }

        public async Task<IEnumerable<CategoryVM>> GetAllAsync()
        {
            try
            {
                var categoryDataAccess = (await unitOfWork.Repository<Category>().GetAsync()).ToList();

                var categorys = mapper.Map<List<Category>, List<CategoryVM>>(categoryDataAccess);
                return categorys;
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

        public async Task<CategoryVM> GetByIdAsync(int id)
        {
            var categoryDataAccess = await unitOfWork.Repository<Category>().FindAsync(id);

            var category = mapper.Map<CategoryVM>(categoryDataAccess);
            return category;
        }

        public async Task<string> Validations(CategoryVM category)
        {
            string validation = string.Empty;

            if ((await unitOfWork.Repository<Category>().GetAsync(x => x.CategoryId != category.CategoryId && x.Name.ToUpper() == category.Name.Trim().ToUpper())).Any())
            {
                validation = "El nombre ingresado ya existe.";
            }

            return validation;
        }
        #endregion
    }
}
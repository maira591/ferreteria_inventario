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

namespace Ferreteria.Domain.SupplierService
{
    public class SupplierService(IUnitOfWork unitOfWork, IMapper mapper, ILogService logService) : ISupplierService
    {
        #region Methods
        public async Task AddOrUpdateAsync(SupplierVM permission)
        {
            const string action = "Supplier AddOrUpdateAsync";

            try
            {
                logService.Add(LogKey.Begin, action);
                var permissionDataAccess = mapper.Map<Supplier>(permission);

                if (permission.SupplierId != default)
                {
                    unitOfWork.Repository<Supplier>().Update(permissionDataAccess);
                }
                unitOfWork.Repository<Supplier>().AddOrUpdate(permissionDataAccess);

                logService.Add(LogKey.Request, JsonConvert.SerializeObject(permissionDataAccess));
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
            bool isUsedPermit = (await unitOfWork.Repository<Product>().GetAsync(x => x.SupplierId == id)).Any();

            if (!isUsedPermit)
            {
                unitOfWork.Repository<Supplier>().Remove(t => t.SupplierId.Equals(id));
                await unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new Exception("El proveedor está asociado a uno o más productos, no se puede eliminar.||");
            }
        }

        public async Task<IEnumerable<SupplierVM>> GetAllAsync()
        {
            try
            {
                var permissionDataAccess = (await unitOfWork.Repository<Supplier>().GetAsync()).ToList();

                var permissions = mapper.Map<List<Supplier>, List<SupplierVM>>(permissionDataAccess);
                return permissions;
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

        public async Task<SupplierVM> GetByIdAsync(int id)
        {
            var permissionDataAccess = await unitOfWork.Repository<Supplier>().FindAsync(id);

            var permission = mapper.Map<SupplierVM>(permissionDataAccess);
            return permission;
        }

        public async Task<string> Validations(SupplierVM permission)
        {
            string validation = string.Empty;

            if ((await unitOfWork.Repository<Supplier>().GetAsync(x => x.SupplierId != permission.SupplierId && x.Name.ToUpper() == permission.Name.Trim().ToUpper())).Any())
            {
                validation = "El nombre ingresado ya existe.";
            }

            return validation;
        }
        #endregion
    }
}
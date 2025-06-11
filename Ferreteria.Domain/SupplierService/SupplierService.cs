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
        public async Task AddOrUpdateAsync(SupplierVM supplier)
        {
            const string action = "Supplier AddOrUpdateAsync";

            try
            {
                logService.Add(LogKey.Begin, action);
                var supplierDataAccess = mapper.Map<Supplier>(supplier);

                if (supplier.SupplierId != default)
                {
                    unitOfWork.Repository<Supplier>().Update(supplierDataAccess);
                }
                unitOfWork.Repository<Supplier>().AddOrUpdate(supplierDataAccess);

                logService.Add(LogKey.Request, JsonConvert.SerializeObject(supplierDataAccess));
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
                var supplierDataAccess = (await unitOfWork.Repository<Supplier>().GetAsync()).ToList();

                var suppliers = mapper.Map<List<Supplier>, List<SupplierVM>>(supplierDataAccess);
                return suppliers;
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
            var supplierDataAccess = await unitOfWork.Repository<Supplier>().FindAsync(id);

            var supplier = mapper.Map<SupplierVM>(supplierDataAccess);
            return supplier;
        }

        public async Task<string> Validations(SupplierVM supplier)
        {
            string validation = string.Empty;

            if ((await unitOfWork.Repository<Supplier>().GetAsync(x => x.SupplierId != supplier.SupplierId && x.Name.ToUpper() == supplier.Name.Trim().ToUpper())).Any())
            {
                validation = "El nombre ingresado ya existe.";
            }

            return validation;
        }
        #endregion
    }
}
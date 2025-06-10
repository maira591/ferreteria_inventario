using AutoMapper;
using Ferreteria.DataAccess.Core;
using Ferreteria.DataAccess.Model;
using Ferreteria.Domain.LogService;
using Ferreteria.Domain.PermissionService;
using Ferreteria.Domain.ViewModel.Security;
using Ferreteria.Infrastructure.Extensions;
using Newtonsoft.Json;
using OpenXmlPowerTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ferreteria.Domain.PermissionService
{
    public class PermissionService(IUnitOfWork unitOfWork, IMapper mapper, ILogService logService) : IPermissionService
    {
        
        #region Methods
        public async Task AddOrUpdateAsync(PermissionVM permission)
        {
            const string action = "Permission AddOrUpdateAsync";

            try
            {
                logService.Add(LogKey.Begin, action);
                var permissionDataAccess = mapper.Map<Permission>(permission);

                if (permission.PermissionId != default)
                {
                    unitOfWork.Repository<Permission>().Update(permissionDataAccess);
                }
                unitOfWork.Repository<Permission>().AddOrUpdate(permissionDataAccess);

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
            bool isUsedPermit = (await unitOfWork.Repository<RolePermission>().GetAsync(x => x.PermissionId == id)).Any();

            if (!isUsedPermit)
            {
                unitOfWork.Repository<Permission>().Remove(t => t.PermissionId.Equals(id));
                await unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new Exception("El permiso está asociado a uno o más roles, no se puede eliminar.||");
            }
        }

        public async Task<IEnumerable<PermissionVM>> GetAllAsync()
        {
            try
            {
                var permissionDataAccess = (await unitOfWork.Repository<Permission>().GetAsync()).ToList();

                var permissions = mapper.Map<List<Permission>, List<PermissionVM>>(permissionDataAccess);
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

        public async Task<PermissionVM> GetByIdAsync(int id)
        {
            var permissionDataAccess = await unitOfWork.Repository<Permission>().FindAsync(id);

            var permission = mapper.Map<PermissionVM>(permissionDataAccess);
            return permission;
        }

        public async Task<string> Validations(PermissionVM permission)
        {
            string validation = string.Empty;

            if ((await unitOfWork.Repository<Permission>().GetAsync(x => x.PermissionId != permission.PermissionId && x.Code.ToUpper() == permission.Code.Trim().ToUpper())).Any())
            {
                validation = "El código ingresado ya existe.";
            }

            return validation;
        }
        #endregion
    }
}
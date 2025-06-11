using AutoMapper;
using Ferreteria.DataAccess.Core;
using Ferreteria.DataAccess.Model;
using Ferreteria.Domain.LogService;
using Ferreteria.Domain.RolePermissionService;
using Ferreteria.Domain.ViewModel.Security;
using Ferreteria.Infrastructure.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ferreteria.Domain.RoleService
{
    public class RoleService(IUnitOfWork unitOfWork, IMapper mapper, ILogService logService, IRolePermissionService rolePermissionService) : IRoleService
    {
        #region Public Methods
        public async Task AddOrUpdateAsync(RoleVM role)
        {
            const string action = "Role AddOrUpdateAsync";

            try
            {
                logService.Add(LogKey.Begin, action);
                var roleDataAccess = mapper.Map<Role>(role);

                if (role.RoleId != default)
                {
                    unitOfWork.Repository<Role>().Update(roleDataAccess);
                    await rolePermissionService.DeleteByRoleAsync(role.RoleId);
                }

                unitOfWork.Repository<Role>().AddOrUpdate(roleDataAccess);
                await unitOfWork.SaveChangesAsync();
                role.RoleId = roleDataAccess.RoleId;

                await rolePermissionService.CreateAsync(new RolePermissionVM() { Role = role});

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
            bool isUsedPermit = (await unitOfWork.Repository<UserRole>().GetAsync(x => x.RoleId == id)).Any();

            if (!isUsedPermit)
            {
                unitOfWork.Repository<Role>().Remove(t => t.RoleId.Equals(id));
                await unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new Exception("El rol se está usando, no se puede eliminar.||");
            }
        }

        public async Task<IEnumerable<RoleVM>> GetAllAsync()
        {
            try
            {
                var roleDataAccess = (await unitOfWork.Repository<Role>().GetAsync("RolePermissions", "RolePermissions.Permission")).ToList();

                var roles = mapper.Map<List<Role>, List<RoleVM>>(roleDataAccess);
                return roles;
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

        public async Task<RoleVM> GetByIdAsync(int id)
        {

            try
            {
                var roleDataAccess = (await unitOfWork.Repository<Role>().GetAsync(x => x.RoleId == id, "RolePermissions")).FirstOrDefault();
                var role = mapper.Map<RoleVM>(roleDataAccess);
                role.Permissions = string.Join(",", role.RolePermissions.Select(x => x.PermissionId));

                return role;

            }
            catch (Exception e)
            {

                throw;
            }
            
        }

        public async Task<string> Validations(RoleVM Role)
        {
            string validation = string.Empty;

            if ((await unitOfWork.Repository<Role>().GetAsync(x => x.RoleId != Role.RoleId && x.Name.ToUpper() == Role.Name.Trim().ToUpper())).Any())
            {
                validation = "El nombre ingresado ya existe.";
            }

            return validation;
        }
        #endregion
    }
}
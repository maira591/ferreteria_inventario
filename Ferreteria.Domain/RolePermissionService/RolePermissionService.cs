using AutoMapper;
using Ferreteria.DataAccess.Core;
using Ferreteria.DataAccess.Model;
using Ferreteria.Domain.LogService;
using Ferreteria.Domain.ViewModel.Security;
using Ferreteria.Infrastructure.Core;
using Ferreteria.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ferreteria.Domain.RolePermissionService
{
    public class RolePermissionService(IUnitOfWork unitOfWork, IMapper mapper, ILogService logService) : IRolePermissionService
    {
        #region Methods

        public async Task<IEnumerable<RolePermissionVM>> CreateAsync(RolePermissionVM rolePermissionVM)
        {
            List<RolePermission> data = GetRolePermissionVMList(rolePermissionVM);
            IEnumerable<RolePermissionVM> result = mapper.Map<List<RolePermission>, List<RolePermissionVM>>(data);
            await Task.Run(() => data.ForEach(element => unitOfWork.Repository<RolePermission>().AddOrUpdate(element)));
            return result;
        }

        public async Task DeleteByRoleAsync(int roleId)
        {
            await Task.Run(() => unitOfWork.Repository<RolePermission>().Remove(v => v.RoleId.Equals(roleId)));
        }

        public async Task<IEnumerable<RolePermissionVM>> GetByPermission(int PermissionId)
        {
            try
            {
                var roles = (await unitOfWork.Repository<RolePermission>().GetAsync(f => f.PermissionId.Equals(PermissionId) && f.Role.IsEnabled, "Role")).ToList();

                var rolesList = mapper.Map<List<RolePermission>, List<RolePermissionVM>>(roles);

                return rolesList;
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

        #endregion

        #region Private Methods
        private static List<RolePermission> GetRolePermissionVMList(RolePermissionVM rolePermissionVM)
        {
            List<RolePermission> data = [];
            RoleVM roleVm = rolePermissionVM.Role;

            string[] permissionsArray = roleVm.Permissions.Split(",");

            if (permissionsArray.Length <= 0)
                throw new ArgumentException("Debe haber al menos una permiso seleccionado");

            permissionsArray.ForEach(element =>
            {
                data.Add(new RolePermission
                {
                    PermissionId =  int.Parse(element.Trim()),
                    RoleId = roleVm.RoleId,
                    Role = null,
                    Permission = null
                });
            });

            return data;
        }
        #endregion
    }
}

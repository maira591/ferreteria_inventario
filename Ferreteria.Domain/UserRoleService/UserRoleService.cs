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

namespace Ferreteria.Domain.UserRoleService
{
    public class UserRoleService(IUnitOfWork unitOfWork, IMapper mapper, ILogService logService) : IUserRoleService
    {
        #region Methods

        public async Task<IEnumerable<UserRoleVM>> CreateAsync(UserRoleVM userRoleVM)
        {
            List<UserRole> data = GetRolePermissionVMList(userRoleVM);
            IEnumerable<UserRoleVM> result = mapper.Map<List<UserRole>, List<UserRoleVM>>(data);
            await Task.Run(() => data.ForEach(element => unitOfWork.Repository<UserRole>().AddOrUpdate(element)));
            return result;
        }

        public async Task DeleteByUserIdAsync(int userId)
        {   
            await Task.Run(() => unitOfWork.Repository<UserRole>().Remove(v => v.UserId.Equals(userId)));
        }

        public async Task<IEnumerable<UserRoleVM>> GetByUserId(int userId)
        {
            try
            {
                var roles = (await unitOfWork.Repository<UserRole>().GetAsync(f => f.UserId.Equals(userId) && f.Role.IsEnabled, "User")).ToList();

                var rolesList = mapper.Map<List<UserRole>, List<UserRoleVM>>(roles);

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
        private static List<UserRole> GetRolePermissionVMList(UserRoleVM userRoleVM)
        {
            List<UserRole> data = [];
            UserVM userVM = userRoleVM.User;

            string[] permissionsArray = userVM.Roles.Split(",");

            if (permissionsArray.Length <= 0)
                throw new ArgumentException("Debe haber al menos un rol seleccionado");

            permissionsArray.ForEach(element =>
            {
                data.Add(new UserRole
                {
                    RoleId =  int.Parse(element.Trim()),
                    UserId = userVM.UserId,
                    Role = null,
                    User = null
                });
            });

            return data;
        }
        #endregion
    }
}

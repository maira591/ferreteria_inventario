using AutoMapper;
using Ferreteria.DataAccess.Core;
using Ferreteria.DataAccess.Model;
using Ferreteria.Domain.LogService;
using Ferreteria.Domain.UserRoleService;
using Ferreteria.Domain.ViewModel.Security;
using Ferreteria.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ferreteria.Domain.UserService
{
    public class UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogService logService, IUserRoleService userRoleService) : IUserService
    {
        #region Public Methods
        public async Task AddOrUpdateAsync(UserVM user)
        {
            const string action = "User AddOrUpdateAsync";

            try
            {
                logService.Add(LogKey.Begin, action);
                var userDataAccess = mapper.Map<User>(user);

                if (user.UserId != default)
                {
                    unitOfWork.Repository<User>().Update(userDataAccess);
                    await userRoleService.DeleteByUserIdAsync(user.UserId);
                }

                unitOfWork.Repository<User>().AddOrUpdate(userDataAccess);
                await unitOfWork.SaveChangesAsync();
                user.UserId = userDataAccess.UserId;

                await userRoleService.CreateAsync(new UserRoleVM() { User = user });

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
            bool isUsedPermit = (await unitOfWork.Repository<UserRole>().GetAsync(x => x.UserId == id)).Any();

            if (!isUsedPermit)
            {
                unitOfWork.Repository<User>().Remove(t => t.UserId.Equals(id));
                await unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new Exception("El usuario se está usando, no se puede eliminar.||");
            }
        }

        public async Task<IEnumerable<UserVM>> GetAllAsync()
        {
            try
            {
                var userDataAccess = (await unitOfWork.Repository<User>().GetAsync("UserRoles", "UserRoles.Role")).ToList();

                var users = mapper.Map<List<User>, List<UserVM>>(userDataAccess);
                return users;
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

        public async Task<UserVM> GetByIdAsync(int id)
        {
            var userDataAccess = (await unitOfWork.Repository<User>().GetAsync(x => x.UserId == id, "UserRoles")).FirstOrDefault();
            var user = mapper.Map<UserVM>(userDataAccess);
            user.Roles = string.Join(",", user.UserRoles.Select(x => x.RoleId));

            return user;
        }

        public async Task<string> Validations(UserVM User)
        {
            string validation = string.Empty;

            if ((await unitOfWork.Repository<User>().GetAsync(x => x.UserId != User.UserId && x.Email.ToUpper() == User.Email.Trim().ToUpper())).Any())
            {
                validation = "El Correo ingresado ya existe.";
            }

            return validation;
        }
        #endregion
    }
}
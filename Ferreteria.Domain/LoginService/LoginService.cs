using AutoMapper;
using Ferreteria.DataAccess.Core;
using Ferreteria.DataAccess.Model;
using Ferreteria.DataAccess.Model.Generics;
using Ferreteria.Domain.LogService;
using Ferreteria.Domain.ViewModel;
using Ferreteria.Domain.ViewModel.Auth;
using Ferreteria.Domain.ViewModel.Login;
using Ferreteria.Infrastructure.Configuration;
using Ferreteria.Infrastructure.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Ferreteria.Domain.LoginService
{
    public class LoginService : ILoginService
    {
        #region PrivateFields

        private readonly IConfigurator _configurationService;
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly string _authApiUrl;
        private readonly string _apitoken;

        #endregion

        #region Constructor

        public LoginService( IConfigurator configurationService,
            ILogService logService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _configurationService = configurationService;
            _logService = logService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authApiUrl = string.Empty;
            _apitoken = string.Empty;
        }

        #endregion

        #region Public Methods

        public async Task<ResponseVM<UserModelComplete>> Login(LoginViewModel model)
        {
            return await LoginValidationAsync(model);
        }

        /// <summary>
        /// Cambiar contraseña
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public GenericResponse<string> ChangePassword(PasswordChangeResquest model)
        {
            const string action = "ChangePassword";
            try
            {
                _logService.Add(LogKey.Begin, action);

                var request = new RequestVM<PasswordChangeResquest>
                {
                    Header = new HeaderVM
                    {
                        ApiToken = Guid.Parse(_apitoken)
                    },
                    Body = model
                };

                var currentRequest = JsonConvert.SerializeObject(request);
                var url = $"{_authApiUrl}Account/Password-Change";
                var apiResponse = string.Empty; //_connectionService.ConsumerToApi(url, "POST", currentRequest);
                var objResponse = JsonConvert.DeserializeObject<GenericResponse<string>>(apiResponse);

                return objResponse;
            }
            catch (Exception e)
            {
                _logService.Add(LogKey.Error, $"Trace: {e.Trace()}, Error Message: {e.Message()}");
                var exceptionResponse = new GenericResponse<string>()
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = e.Message()
                };
                return exceptionResponse;
            }
            finally
            {
                _logService.Add(LogKey.End, action);
                _logService.Generate();
            }
        }


        public ResponseVM<PasswordPoliciesVM> PasswordPolicies()
        {
            const string action = "Get Password Policices";
            var response = new ResponseVM<PasswordPoliciesVM>();
            try
            {
                _logService.Add(LogKey.Begin, action);
                var apiToken = _configurationService.GetKey("ApiToken");
                var request = new RequestVM<PasswordPolciciesRequest>
                {
                    Header = new HeaderVM
                    {
                        ApiToken = Guid.Parse(apiToken)
                    }
                };

                var currentRequest = JsonConvert.SerializeObject(request);
                var url = $"{_authApiUrl}Account/PasswordPolicies";

                var apiResponse = string.Empty; //_connectionService.ConsumerToApi(url, "POST", currentRequest);

                if (apiResponse.StartsWith("<"))
                {
                    response.Message = $"Respuesta inválida de Api:\n{apiResponse}";
                    response.Status = HttpStatusCode.InternalServerError;
                    return response;
                }

                response = JsonConvert.DeserializeObject<ResponseVM<PasswordPoliciesVM>>(apiResponse);

                return response;
            }
            catch (Exception e)
            {
                _logService.Add(LogKey.Error, $"Trace: {e.Trace()}, Error Message: {e.Message()}");
                response.Message = $"Ocurrió un error en el proceso.\n{e.Message()}";
                response.Status = HttpStatusCode.InternalServerError;
                return response;
            }
            finally
            {
                _logService.Add(LogKey.End, action);
                _logService.Generate();
            }
        }
        #endregion

        #region Private Methods
        private static ResponseVM<string> SetExceptionResponse(string message)
        {
            var exceptionResponse = new ResponseVM<string>
            {
                Body = message,
                Message = message,
                Status = HttpStatusCode.InternalServerError
            };
            return exceptionResponse;
        }


        private async Task<ResponseVM<UserModelComplete>> LoginValidationAsync(LoginViewModel model)
        {
            string userName = model.Username;
            string pass = model.Password;

            User userAccount = (await _unitOfWork.Repository<User>().GetAsync(x => x.Email.ToLower() == userName.ToLower())).FirstOrDefault();

            if (userAccount == null)
            {
                return ResponseLogin(HttpStatusCode.BadRequest, "El usuario no existe", null);
            }

            if (userAccount != null && !userAccount.IsEnabled)
            {
                return ResponseLogin(HttpStatusCode.BadRequest, "El usuario está deshabilitado", null);
            }

            if (userAccount.IsLocked)
            {
                return ResponseLogin(HttpStatusCode.BadRequest, "Usuario bloqueado", null);
            }

            if (!userAccount.Password.Equals(pass))
            {
                userAccount.FailedAttempts = userAccount.FailedAttempts + 1;
            }

            int accessFailedCount = 5;

            if (userAccount.FailedAttempts > 0 && !userAccount.Password.Equals(pass))
            {
                if (userAccount.FailedAttempts >= accessFailedCount)
                {
                    _unitOfWork.Repository<User>().Update(userAccount);
                    _unitOfWork.SaveChanges();
                    return ResponseLogin(HttpStatusCode.BadRequest, "Usuario bloqueado", null);
                }
                else
                {
                    _unitOfWork.Repository<User>().Update(userAccount);
                    _unitOfWork.SaveChanges();

                    return ResponseLogin(HttpStatusCode.BadRequest, $"Credenciales incorrectas. Tiene {accessFailedCount - userAccount.FailedAttempts} intentos restantes", null);
                }
            }

            var listRolesId = (await _unitOfWork.Repository<UserRole>().GetAsync(x => x.UserId == userAccount.UserId)).Select(x => x.RoleId).ToList();
            var hasRoles = (from ur in await _unitOfWork.Repository<UserRole>()
                .GetAsync(x => x.UserId == userAccount.UserId)
                            join r in listRolesId on ur.RoleId equals r
                            select ur).Any();

            if (!hasRoles)
            {
                return ResponseLogin(HttpStatusCode.BadRequest, "Usuario sin roles.", null);
            }

            //Si no se dispara ninguna validación, el login es correcto y se restablecen los intentos de inicio de sesión.
            userAccount.FailedAttempts = 0;
            userAccount.IsLocked = false;

            _unitOfWork.Repository<User>().Update(userAccount);
            await _unitOfWork.SaveChangesAsync();

            var userModelComplete = await GetUserModel(userAccount);

            return ResponseLogin(HttpStatusCode.OK, "OK", userModelComplete);
        }

        private async Task<UserModelComplete> GetUserModel(User user)
        {
            if (user == null)
            {
                return null;
            }

            var roles = (await _unitOfWork.Repository<UserRole>().GetAsync(x => x.UserId == user.UserId, "Role")).ToList();
            var privigeles = (from rp in await _unitOfWork.Repository<RolePermission>().GetAsync("Permission")
                              join r in roles on rp.RoleId equals r.RoleId
                              select rp).ToList();


            var resultUser = _mapper.Map<User, UserModelComplete>(user);
            var currentRolesList = new List<ViewModel.Login.RoleModel>();

            foreach (var rol in roles)
            {
                var currentRoles = new ViewModel.Login.RoleModel
                {
                    Description = rol.Role.Name,
                    Enabled = rol.Role.IsEnabled,
                    Id = rol.Role.RoleId,
                    Name = rol.Role.Name,
                    Privileges = GetPrivileges(rol.Role.RoleId, privigeles),
                };

                currentRolesList.Add(currentRoles);
            }

            resultUser.Roles = currentRolesList;

            return resultUser;
        }

        private List<ViewModel.Login.PrivilegeModel> GetPrivileges(int rolId, ICollection<RolePermission> rolePrivileges, bool showDisabled = false)
        {
            var privilegeModelList = new List<ViewModel.Login.PrivilegeModel>();
            var rolPrivilegesList = rolePrivileges.Where(s => s.RoleId == rolId);
            foreach (var pm in rolPrivilegesList)
            {
                var privilegeModel = new Ferreteria.Domain.ViewModel.Login.PrivilegeModel
                {
                    Code = pm.Permission.Code,
                    Enabled = pm.Permission.IsEnabled,
                    Id = pm.Permission.PermissionId,
                    Name = pm.Permission.Name
                };
                if (privilegeModel.Enabled || showDisabled)
                    privilegeModelList.Add(privilegeModel);
            }
            return privilegeModelList;
        }

        private ResponseVM<UserModelComplete> ResponseLogin(HttpStatusCode httpStatusCode, string message, UserModelComplete userModel)
        {
            if (httpStatusCode == HttpStatusCode.BadRequest)
            {
                _logService.Add(LogKey.Msg, message);
            }

            return new ResponseVM<UserModelComplete>()
            {
                Status = httpStatusCode,
                Body = userModel,
                Message = message
            };
        }
        #endregion
    }
}

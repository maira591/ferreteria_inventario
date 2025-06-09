using Ferreteria.DataAccess.Model.Generics;
using Ferreteria.Domain.ViewModel;
using Ferreteria.Domain.ViewModel.Auth;
using Ferreteria.Domain.ViewModel.Login;
using Ferreteria.Domain.ViewModel.Security;
using System.Threading.Tasks;

namespace Ferreteria.Domain.LoginService
{
    public interface ILoginService
    {
        Task<ResponseVM<UserModelComplete>> Login(LoginViewModel model);
        GenericResponse<string> ChangePassword(PasswordChangeResquest model);
        ResponseVM<PasswordPoliciesVM> PasswordPolicies();
    }
}

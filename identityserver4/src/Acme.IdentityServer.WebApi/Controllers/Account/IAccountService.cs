using System.Threading.Tasks;

namespace Acme.IdentityServer.WebApi.Controllers.Account {
    public interface IAccountService {
        Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl, string forgotPasswordAddress);

        Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model);

        Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId);

        Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId);
    }
}

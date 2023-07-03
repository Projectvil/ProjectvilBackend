using WebGateway.ViewModels.Auth;

namespace WebGateway.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<TokenResponse> Login(LoginViewModel loginModel);
        public Task<TokenResponse> SignUp(SignUpViewModel signUpViewModel);

    }
}

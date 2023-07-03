using AuthMicroservice.Services.Responses;

namespace AuthMicroservice.Services.Interfaces;

public interface IAuthService
{
    public Task<AuthServiceResponse> Login(string email, string password);
    public Task<AuthServiceResponse> SignUp(string email, string password, string name);

}
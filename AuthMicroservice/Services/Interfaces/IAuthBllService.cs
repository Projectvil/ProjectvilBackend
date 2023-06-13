using AuthMicroservice.Services.Responses;

namespace AuthMicroservice.Services.Interfaces;

public interface IAuthBllService
{
    public Task<AuthServiceResponse> Login(string email, string password);
    public Task<SignUpResult> SignUp(string email, string password, string name);

}
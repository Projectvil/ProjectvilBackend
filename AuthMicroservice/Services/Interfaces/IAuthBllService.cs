using AuthMicroservice.Services.Responses;

namespace AuthMicroservice.Services.Interfaces;

public interface IAuthBllService
{
    public Task<AuthServiceResponse> Login(string email, string password);
}
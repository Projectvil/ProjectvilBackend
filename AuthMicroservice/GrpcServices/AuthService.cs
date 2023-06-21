using AuthMicroservice.Services.Implementations;
using AuthMicroservice.Services.Interfaces;
using AuthMicroservice.Services.Responses;
using Grpc.Core;

namespace AuthMicroservice.GrpcServices;

public class AuthService : AuthMicroservice.AuthService.AuthServiceBase
{
    private readonly ILogger<AuthService> _logger;
    private readonly IAuthBllService _authBllService;


    public AuthService(ILogger<AuthService> logger, 
        IAuthBllService authBllService
        )
    {
        _logger = logger;
        _authBllService = authBllService;
    }

    public override async Task<TokenResponse> Login(LoginRequest request, ServerCallContext context)
    {
        _logger.LogWarning($"AuthMicroservice : Login endpoint with email {request.Email}");

        var tokens = await _authBllService.Login(request.Email, request.Password);
        if (tokens.Success)
        {
            
            return new TokenResponse()
            {
                AccessToken = tokens.TokenResponse.AccessToken,
                RefreshToken = tokens.TokenResponse.RefreshToken
            };
        }


        if (tokens.Error == Errors.InvalidPassword)
        {

            throw new RpcException(new Status(StatusCode.InvalidArgument, "invalid_password"));
        }

        if (tokens.Error == Errors.UserNotFound) 
        {
            throw new RpcException(new Status(StatusCode.NotFound, "user_not_found"));
        }

        return new TokenResponse();
    }

    public override async Task<SignUpResult> SignUp(SignUpRequest request, ServerCallContext context)
    {
        var result = await _authBllService.SignUp(request.Email, request.Password, request.Name);

        return result;
    }
}
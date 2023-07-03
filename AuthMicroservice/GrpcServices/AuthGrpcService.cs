using AuthMicroservice.Services.Implementations;
using AuthMicroservice.Services.Interfaces;
using AuthMicroservice.Services.Responses;
using Grpc.Core;

namespace AuthMicroservice.GrpcServices;

public class AuthGrpcService : AuthMicroservice.AuthGrpcService.AuthGrpcServiceBase
{
    private readonly ILogger<AuthGrpcService> _logger;
    private readonly IAuthService _authService;


    public AuthGrpcService(ILogger<AuthGrpcService> logger, 
        IAuthService authService
        )
    {
        _logger = logger;
        _authService = authService;
    }

    public override async Task<TokenResponse> Login(LoginRequest request, ServerCallContext context)
    {
        _logger.LogWarning($"AuthMicroservice : Login endpoint with email {request.Email}");

        var tokens = await _authService.Login(request.Email, request.Password);
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

    public override async Task<TokenResponse> SignUp(SignUpRequest request, ServerCallContext context)
    {
        var result = await _authService.SignUp(request.Email, request.Password, request.UserName);

        if (result.Success)
        {
            return result.TokenResponse;
        }

        throw new RpcException(new Status(StatusCode.NotFound, result.ErrorMessage));
    }
}
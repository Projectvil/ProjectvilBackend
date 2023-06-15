using AuthMicroservice.Services.Implementations;
using AuthMicroservice.Services.Interfaces;
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
        _logger.LogWarning($"AuthMicroservice : Login endpoint {request.ToString()}");


        var tokens = await _authBllService.Login(request.Email, request.Password);
        if (tokens.Success)
        {
            
            return new TokenResponse()
            {
                AccessToken = tokens.TokenResponse.AccessToken,
                RefreshToken = tokens.TokenResponse.RefreshToken
            };
        }
        
        var metadata = new Metadata
        {
            { "User",  "aaa"}
        };
        
        throw new RpcException(new Status(StatusCode.PermissionDenied, "Permission denied"), metadata);
    }

    public override async Task<SignUpResult> SignUp(SignUpRequest request, ServerCallContext context)
    {
        await _authBllService.SignUp(request.Email, request.Password, request.Name);

        return new SignUpResult();
    }
}
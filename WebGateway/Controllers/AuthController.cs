using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;


namespace WebGateway.Controllers;

[ApiController]
[Route("auth/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        TokenResponse result = new TokenResponse();
        
        try
        {
            using var channel = GrpcChannel.ForAddress("http://auth_microservice:5012");

            var client = new AuthService.AuthServiceClient(channel);
            result = await client.LoginAsync(new LoginRequest(){Email = "admin@gmail.com", Password = "Admin123*"});
            
        }
        catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.PermissionDenied)
        {
            return BadRequest();
        }
        catch (RpcException)
        {
            // Handle any other error type ...
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpRequest signUpReq) 
    {
        SignUpResult result = new SignUpResult();

        try
        {
            using var channel = GrpcChannel.ForAddress("http://auth_microservice:5012");

            var client = new AuthService.AuthServiceClient(channel);
            result = await client.SignUpAsync(new SignUpRequest() { Email = "admin@gmail.com", Password = "Admin123*", Name = "Alex"});

        }
        catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.PermissionDenied)
        {
            return BadRequest();
        }
        catch (RpcException)
        {
            // Handle any other error type ...
        }

        return Ok(result);
    }
}
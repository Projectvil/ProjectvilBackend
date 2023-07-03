using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using WebGateway.Services.Implementations;
using WebGateway.Services.Interfaces;
using WebGateway.ViewModels.Auth;

namespace WebGateway.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;
    public AuthController(ILogger<AuthController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [Route("login")]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {//
        _logger.LogWarning(message: $"Login endpoint: email:{loginViewModel.Email}, password:{loginViewModel.Password}");

        TokenResponse tokenResponse;

        try
        {
            tokenResponse = await _authService.Login(loginViewModel);

        }
        catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.PermissionDenied)
        {
            return BadRequest();
        }
        catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
        {
            return NotFound(ex.Status.Detail);
        }
        catch (RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.InvalidArgument) 
        {
            return BadRequest(ex.Status.Detail);   
        }


        return Ok(tokenResponse);
    }

    [Route("sign_up")]
    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpViewModel signUpReq) 
    {
        TokenResponse result = new TokenResponse();

        try
        { 
            result = await _authService.SignUp(signUpReq);
        }
        catch (RpcException ex)
        {
            return BadRequest(ex.Status.Detail);
        }

        return Ok(result);
    }
}
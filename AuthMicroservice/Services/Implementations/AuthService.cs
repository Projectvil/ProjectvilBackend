using AuthMicroservice.Data.Models;
using AuthMicroservice.Services.Interfaces;
using AuthMicroservice.Services.Responses;
using Microsoft.AspNetCore.Identity;

namespace AuthMicroservice.Services.Implementations;

public class AuthBllService : IAuthBllService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokensService _tokensService;
    
    public AuthBllService(UserManager<User> userManager, ITokensService tokensService)
    {
        _userManager = userManager;
        _tokensService = tokensService;
    }
    
    public async Task<AuthServiceResponse> Login(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new AuthServiceResponse()
            {
                Success = false,
                ErrorMessage = "user_not_found",
                Error = Errors.UserNotFound
            };
        }

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
        
        if (!isPasswordCorrect)
        {
            return new AuthServiceResponse()
            {
                Success = false,
                ErrorMessage = "invalid_password",
                Error = Errors.InvalidPassword
            };
        }

        var tokens = await _tokensService.GenerateTokens(user);
        
        return new AuthServiceResponse()
        {
            TokenResponse = new TokenResponse()
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken
            }
        };

    }

    public async Task<SignUpResult> SignUp(string email, string password, string name)
    {
        var user = new User() { Email = email, UserName = name };
        var registrationResult = await _userManager.CreateAsync(user, password);
        var userId = await _userManager.GetUserIdAsync(user);

        if (registrationResult.Succeeded)
        {
            return new SignUpResult()
            {
                UserId = userId
            };
        }

        return new SignUpResult()
        {
            ErrorMessage = registrationResult.Errors.ToString()
        };
    }
}
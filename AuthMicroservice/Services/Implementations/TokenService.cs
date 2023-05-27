using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using AuthMicroservice.Data.Models;
using AuthMicroservice.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AuthMicroservice.Services.Implementations;

public class TokenService : ITokensService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;

    public TokenService(RoleManager<IdentityRole> roleManager, 
        UserManager<User> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    private const int AccessTokenExpiresMinutes = 60;
    private const int RefreshTokenExpiresDays = 30;
    

    public async Task<TokenResponse> GenerateTokens(User candidateForTokens)
    {
        var claims = await GetUserClaims(candidateForTokens);
        var accessToken = GenerateAccessToken(claims);
        var refreshToken = GenerateRefreshToken(claims);

        var tokenResponse = new TokenResponse()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };

        return tokenResponse;

    }

    public string GenerateAccessToken(IEnumerable<Claim> userClaims)
    {
        var jwt = new JwtSecurityToken(
                issuer: AuthOptions.Issuer,
                audience: AuthOptions.Audience,
                claims: userClaims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(AccessTokenExpiresMinutes)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public string GenerateRefreshToken(IEnumerable<Claim> userClaims)
    {
        var id = userClaims.Where(claim => claim.Type == "Id");
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.Issuer,
            audience: AuthOptions.Audience,
            claims: userClaims.Where(claim => claim.Type == "Id"),
            expires: DateTime.UtcNow.Add(TimeSpan.FromDays(RefreshTokenExpiresDays)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public async Task<IEnumerable<Claim>> GetUserClaims(User candidateForTokens)
    {
        var userRole = await _userManager.GetRolesAsync(candidateForTokens);
        
        return new List<Claim>() {
                new Claim("Id", candidateForTokens.Id.ToString()),
                new Claim(ClaimTypes.Role, userRole.First()),
                new Claim("Name", candidateForTokens.FirstName),
            };

    }

    public bool ValidateRefreshToken(string refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters()
        {
            ValidateLifetime = false, // Because there is no expiration in the generated token
            ValidateAudience = false, // Because there is no audiance in the generated token
            ValidateIssuer = false,
            ValidIssuer = AuthOptions.Issuer,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidAudience = AuthOptions.Audience,

        };


        SecurityToken validatedToken;

        try
        {
            IPrincipal principal = tokenHandler.ValidateToken(refreshToken, validationParameters, out validatedToken);

        }
        catch (SecurityTokenSignatureKeyNotFoundException)
        {
            return false;
        }

        return true;
    }
}

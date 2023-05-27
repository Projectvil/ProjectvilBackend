using System.Security.Claims;
using AuthMicroservice.Data.Models;

namespace AuthMicroservice.Services.Interfaces;

public interface ITokensService
{
    Task<TokenResponse> GenerateTokens(User candidateForTokens);
    Task<IEnumerable<Claim>>  GetUserClaims(User candidateForTokens);
    string GenerateAccessToken(IEnumerable<Claim> userClaims);
    string GenerateRefreshToken(IEnumerable<Claim> userClaims);
    bool ValidateRefreshToken(string refreshToken);
}
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthMicroservice.Services;

public class AuthOptions
{
    public const string Issuer = "ProjectvilMicroservice.AuthMicroservice"; 
    public const string Audience = "Projectvil.Microservices";
    const string Key = "mysupersecret_secretkey!123";   
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
}
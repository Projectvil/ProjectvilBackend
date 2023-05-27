using System.Security.Claims;
using AuthMicroservice.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthMicroservice.Data.DbSeed;

public class DbSeed : IDbSeed
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private const string AdminRole = "Admin";
    private const string UserRole = "User";
    
    public DbSeed(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public void Seed()
    {
        if (_roleManager.FindByIdAsync(AdminRole).Result == null)
        {
            _roleManager.CreateAsync(new IdentityRole(AdminRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(UserRole)).GetAwaiter().GetResult();

        }
        else
        {
            return;
        }
        

        var admin = new User()
        {
            UserName = "admin",
            FirstName = "adminFirst",
            LastName = "adminLast",
            Email = "admin@gmail.com",
            EmailConfirmed = true,
        };

        _userManager.CreateAsync(admin, "Admin123*").GetAwaiter().GetResult();
        _userManager.AddToRoleAsync(admin, AdminRole).GetAwaiter().GetResult();

    }
}

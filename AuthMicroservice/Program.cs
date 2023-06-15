using AuthMicroservice.Data;
using AuthMicroservice.Data.DbSeed;
using AuthMicroservice.Data.Models;
using AuthMicroservice.Extensions;
using AuthMicroservice.GrpcServices;
using AuthMicroservice.Services;
using AuthMicroservice.Services.Implementations;
using AuthMicroservice.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddIdentity<User, IdentityRole>(config => 
    {
        config.Password.RequiredLength = 8;
        config.Password.RequireDigit = true;
        config.Password.RequireNonAlphanumeric = true;
        config.Password.RequireUppercase = true;
        config.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        config.Password.RequireLowercase = false;
        config.User.RequireUniqueEmail = true;
        config.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
    })
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthBllService, AuthBllService>();
builder.Services.AddScoped<IDbSeed, DbSeed>();
builder.Services.AddScoped<ITokensService, TokenService>();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSeq(builder.Configuration.GetSection("Seq"));
});


var dbConnectionString = builder.Configuration.GetConnectionString("DockerPostgres");
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(dbConnectionString));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcService<AuthMicroservice.GrpcServices.AuthService>();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<AuthDbContext>();
    context.Database.Migrate();
}
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

using (var scope = scopeFactory.CreateScope())
{
    var dbSeed = scope.ServiceProvider.GetService<IDbSeed>();
    dbSeed?.Seed();
}


app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
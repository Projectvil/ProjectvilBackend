using AutoMapper;
using Microsoft.Extensions.Configuration;
using WebGateway;
using WebGateway.GRPCInteraction;
using WebGateway.Services.Implementations;
using WebGateway.Services.Interfaces;



var builder = WebApplication.CreateBuilder(args);


var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .Build();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IChannelsService, ChannelsService>();
builder.Services.AddScoped<IAuthService, AuthService>();

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);



if (!builder.Environment.IsProduction() || !builder.Environment.IsDevelopment()) 
{
    builder.Services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.AddSeq(config.GetSection("Seq"));
    });
}



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
/*
app.UseHttpsRedirection();*/

app.UseAuthorization();

app.MapControllers();

app.Run();
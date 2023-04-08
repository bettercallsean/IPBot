using System.Text;
using IPBot.AnimeAnalyser;
using IPBot.API.Shared.Services;
using IPBot.DataServices.AutoMapper;
using IPBot.DataServices.Data;
using IPBot.DataServices.DataServices;
using IPBot.DataServices.Interfaces.DataServices;
using IPBot.DataServices.Service;
using IPBot.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

DotEnvHelper.Load(Constants.CredentialsFile);

builder.Configuration.AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IAnimeAnalyser, AnimeAnalyserService>();

RegisterAutoMapperProfiles();
RegisterDataServices();
RegisterServices();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(DotEnvHelper.EnvironmentVariables["SECURITY_KEY_TOKEN"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<IIPBotDataContext, IPBotDbContext>(
    options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

void RegisterDataServices()
{
    builder.Services.AddScoped<IUserDataService, UserDataService>();
    builder.Services.AddScoped<IGameServerDataService, GameServerDataService>();
    builder.Services.AddScoped<IGameDataService, GameDataService>();
}

void RegisterServices()
{
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IGameService, GameService>();
}

void RegisterAutoMapperProfiles()
{
    builder.Services.AddAutoMapper(config =>
    {
        config.AddProfile<GameProfile>();
        config.AddProfile<ServerInfoProfile>();
    });
}
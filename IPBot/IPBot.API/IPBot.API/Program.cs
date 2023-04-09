using System.Text;
using IPBot.API.Business.AutoMapper;
using IPBot.API.Business.Service;
using IPBot.DataServices.Data;
using IPBot.DataServices.DataServices;
using IPBot.DataServices.Interfaces.DataServices;
using IPBot.Shared.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

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
                .GetBytes(builder.Configuration["SecurityKeyToken"])),
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
    builder.Services.AddScoped<IDomainDataService, DomainDataService>();
}

void RegisterServices()
{
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IGameService, GameService>();
    builder.Services.AddScoped<IIPService, IPService>();
    builder.Services.AddSingleton<IAnimeAnalyserService, AnimeAnalyserService>();
}

void RegisterAutoMapperProfiles()
{
    builder.Services.AddAutoMapper(config =>
    {
        config.AddProfile<GameProfile>();
        config.AddProfile<GameServerProfile>();
    });
}
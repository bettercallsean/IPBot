using System.Net;

var _ip = string.Empty;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/updateIP", (string ip) =>
{
    if (!IPAddress.TryParse(ip, out var validIp) || ip.Equals(_ip))
        return "Error";

    _ip = ip;

    return "Success";
})
.WithName("UpdateIP")
.WithOpenApi();

app.MapGet("/getIP", () => _ip)
.WithName("GetIP")
.WithOpenApi();

app.Run();
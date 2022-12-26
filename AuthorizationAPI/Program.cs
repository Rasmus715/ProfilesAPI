using AuthorizationAPI.Commands;
using AuthorizationAPI.Handlers;
using AuthorizationAPI.Infrastructure;
using AuthorizationAPI.Infrastructure.Security;
using AuthorizationAPI.Models;
using MediatR;
using Raven.DependencyInjection;
using Raven.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddHttpClient<IRequestHandler<RegisterCommand>, RegisterHandler>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ExternalAPIs:Profiles")!);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
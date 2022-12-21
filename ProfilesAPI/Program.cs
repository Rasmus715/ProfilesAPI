using System.Reflection;
using AutoMapper;
using MediatR;
using ProfilesAPI;
using ProfilesAPI.Behaviours;
using ProfilesAPI.Data;
using ProfilesAPI.Models;
using ProfilesAPI.RabbitMq;
using ProfilesAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Add services to the container.
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.Configure<PersistenceSettings>(builder.Configuration.GetSection("Database"));
builder.Services.AddSingleton<IRavenContext, RavenContext>();
builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();
builder.Services.AddSingleton(typeof(IRavenRepository<>), typeof(RavenRepository<>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddHttpClient("OfficesAPI", httpClient =>
{
    httpClient.BaseAddress = new Uri("http://localhost:5055/api/Offices/");
});

builder.Services.AddControllers();

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

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

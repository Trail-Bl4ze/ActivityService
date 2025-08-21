using System.Diagnostics;
using ActivityService.App.BackgroundJobs;
using ActivityService.App.Interfaces;
using ActivityService.Domain;
using ActivityService.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация
var configuration = builder.Configuration;

// Добавление сервисов
builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
    
builder.Services.AddDbContext<ActivityDbContext>(options =>
    options.UseNpgsql(connectionString));

// Регистрация сервисов приложения
//builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddScoped<IActivityService, ActivityService.App.Services.ActivityService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 16 * 1024 * 1024; // 16MB
});

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseRouting();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<ActivitiesGrpcService>();
app.MapGet("/", () => "gRPC server is running. Use a gRPC client to communicate.");

app.Run();
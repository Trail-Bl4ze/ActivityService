using System.Diagnostics;
using ActivityService.App.Interfaces;
using ActivityService.Domain;
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
builder.Services.AddScoped<IActivityService, ActivityService.App.Services.ActivityService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
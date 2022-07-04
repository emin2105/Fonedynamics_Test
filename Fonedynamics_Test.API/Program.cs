using Fonedynamics_Test.API.Endpoints;
using Fonedynamics_Test.API.Services;
using Fonedynamics_Test.Shared.Data;
using Fonedynamics_Test.Shared.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

IConfiguration configuration = new ConfigurationBuilder()
   .AddJsonFile("appsettings.json")
   .AddEnvironmentVariables()
   .AddCommandLine(args)
   .Build();

builder.Services.AddScoped<SMSService>();

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin();
            }));

var connectionString = configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<SMSDbContext>(options => options.UseSqlServer(connectionString, x => x.MigrationsAssembly("Fonedynamics_Test.Shared"))).BuildServiceProvider();
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((cntx, cnfg) =>
    {
        cnfg.Host(configuration.GetValue<string>("RabbitMqUrl"));
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.MapEndpoints();

app.Run();

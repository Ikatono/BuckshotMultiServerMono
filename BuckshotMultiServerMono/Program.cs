using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GameController API" });
});

builder.Services.AddControllers();

var app = builder.Build();
app.UseWebSockets();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GameController API V1");
});

//app.MapPost("/api/v1/newgame", () =>
//{

//});



app.Run();

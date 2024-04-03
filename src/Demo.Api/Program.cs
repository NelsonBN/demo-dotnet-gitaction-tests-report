using Demo.Infrastructure;
using Demo.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services
    .AddApplicationLayer()
    .AddInfrastructureLayer();

var app = builder.Build();

app.UseSwagger()
   .UseSwaggerUI();

app.MapProductsEndpoints();

app.Run();

public sealed class Bootstrap;
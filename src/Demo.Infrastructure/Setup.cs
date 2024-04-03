﻿using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Demo.Application.Services;
using Dapper;

namespace Demo.Infrastructure;

public static class Setup
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services)
    {
        SqlMapper.AddTypeHandler(new GuidTypeHandlerMapper());

        return services
            .AddScoped<IProductsRepository, ProductsRepository>()
            .AddScoped<IDbConnection>(sp =>
            {
                var connectionString = sp.GetRequiredService<IConfiguration>()
                    .GetConnectionString("Default");

                var connection = new MySqlConnection(connectionString);
                connection.Open();

                return connection;
            });
    }
}

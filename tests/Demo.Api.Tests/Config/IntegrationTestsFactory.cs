using Meziantou.Extensions.Logging.Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System.Data;
using Xunit.Abstractions;

namespace Demo.Api.Tests.Config;

public sealed partial class IntegrationTestsFactory : WebApplicationFactory<Bootstrap>
{
    public ITestOutputHelper Output { get; set; } = default!;

    public IntegrationTestsFactory()
        => InitMySQL();


    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => builder
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Error);
                logging.AddFilter(_ => true);
                logging.Services.AddSingleton<ILoggerProvider>(serviceProvider => new XUnitLoggerProvider(Output));
            })
            .ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(IDbConnection));
                services.AddScoped<IDbConnection>(sp =>
                {
                    var connection = new MySqlConnection(_connectionString);
                    connection.Open();

                    return connection;
                });
            });
}

[CollectionDefinition(nameof(CollectionIntegrationTests))]
public sealed class CollectionIntegrationTests : ICollectionFixture<IntegrationTestsFactory> { }

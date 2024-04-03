using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Api.Tests.Config;

public sealed partial class IntegrationTestsFactory : IAsyncLifetime
{
    private const string MYSQL_DATABASE = "demo";
    private const string MYSQL_USERNAME = "root";
    private const string MYSQL_ROOT_PASSWORD = "testpassword";
    private const int MYSQL_CONTAINER_PORT = 3306;
    private IContainer _mySQLContainer = default!;

    private void InitMySQL()
        => _mySQLContainer = new ContainerBuilder()
            .WithImage("mysql:8.3.0")
            .WithEnvironment("MYSQL_ROOT_PASSWORD", MYSQL_ROOT_PASSWORD)
            .WithEnvironment("MYSQL_DATABASE", MYSQL_DATABASE)
            .WithPortBinding(MYSQL_CONTAINER_PORT, true)
            .WithResourceMapping(Path
                .GetFullPath("./Data"), "/docker-entrypoint-initdb.d")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(MYSQL_CONTAINER_PORT))
            .Build();

    public async Task InitializeAsync()
        => await _mySQLContainer.StartAsync();

    public async Task DisposeAsync()
        => await _mySQLContainer.StopAsync();

    private string _connectionString
       => $"server={_mySQLContainer.Hostname};Port={_mySQLContainer.GetMappedPublicPort(MYSQL_CONTAINER_PORT)};database={MYSQL_DATABASE};uid={MYSQL_USERNAME};password={MYSQL_ROOT_PASSWORD};pooling=true;";
}

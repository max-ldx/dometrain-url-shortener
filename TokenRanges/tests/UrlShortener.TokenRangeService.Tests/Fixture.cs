using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using Testcontainers.PostgreSql;

namespace UrlShortener.TokenRangeService.Tests;

public class Fixture : WebApplicationFactory<ITokenRangeAssemblyMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder("postgres:18-alpine")
        .Build();

    private string ConnectionString => _postgresContainer.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        Environment.SetEnvironmentVariable("Postgres__ConnectionString", ConnectionString);

        await InitializeSqlTable();
    }

    public new async Task DisposeAsync()
    {
        await _postgresContainer.StopAsync();
        await base.DisposeAsync();
    }

    private async Task InitializeSqlTable()
    {
        var tableSql = await File.ReadAllTextAsync("Table.sql");

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(tableSql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
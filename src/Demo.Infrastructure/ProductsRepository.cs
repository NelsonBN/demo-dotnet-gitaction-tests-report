using Dapper;
using Demo.Application.Services;
using Demo.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Infrastructure;

internal sealed class ProductsRepository(IDbConnection connection) : IProductsRepository
{
    private readonly IDbConnection _connection = connection;

    public async Task<IEnumerable<Product>> ListAsync(CancellationToken cancellationToken = default)
        => await _connection.QueryAsync<Product>(
            """
            SELECT
                `Id`,
                `Name`,
                `Quantity`
            FROM `Product` ;
            """);

    public async Task<Product?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => await _connection.QuerySingleOrDefaultAsync<Product>(
            """
            SELECT
                `Id`,
                `Name`,
                `Quantity`
            FROM `Product`
            WHERE `Id` = @id ;
            """,
            new { id });

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        => await _connection.ExecuteAsync(
            """
                INSERT `Product` (`Id`, `Name`, `Quantity`)
                           VALUE (@Id , @Name , @Quantity );
                """,
                product);

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
        => await _connection.ExecuteAsync(
        """
            UPDATE `Product`
               SET `Name` = @Name,
                   `Quantity` = @Quantity
            WHERE `id` = @Id ;
            """,
            product);

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await _connection.ExecuteAsync(
            """
            Delete FROM `Product`
            WHERE `Id` = @id ;
            """,
            new { id });

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        => await _connection.ExecuteScalarAsync<bool>(
        """
            SELECT EXISTS(
                SELECT 1
                FROM `Product`
                WHERE `Id` = @id
            );
            """,
            new { id });


    public async Task<bool> ExistsAsync(string? name, CancellationToken cancellationToken = default)
        => await _connection.ExecuteScalarAsync<bool>(
        """
            SELECT EXISTS(
                SELECT 1
                FROM `Product`
                WHERE `Name` = @name
            );
            """,
            new { name });

    public async Task<bool> ExistsAsync(Guid id, string? name, CancellationToken cancellationToken = default)
        => await _connection.ExecuteScalarAsync<bool>(
        """
            SELECT EXISTS(
                SELECT 1
                FROM `Product`
                WHERE `Id` != @id
                  AND `Name` = @name
            );
            """,
            new 
            { 
                id,
                name
            });
}

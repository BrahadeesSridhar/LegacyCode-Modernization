using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using LegacyCodeModernization.Models;
using LegacyCodeModernization.Interfaces;
using Microsoft.Extensions.Logging;
using Dapper;

namespace LegacyCodeModernization.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(IConfiguration configuration, ILogger<ProductRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Product> GetProductAsync(Guid productId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = "SELECT Id, Name, Stock FROM Products WHERE Id = @ProductId";
                return await connection.QuerySingleOrDefaultAsync<Product>(sql, new { ProductId = productId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID: {ProductId}", productId);
                throw;
            }
        }

        public async Task<bool> UpdateStockAsync(Guid productId, int quantity)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    UPDATE Products 
                    SET Stock = Stock - @Quantity 
                    WHERE Id = @ProductId AND Stock >= @Quantity";

                var rowsAffected = await connection.ExecuteAsync(sql, new { ProductId = productId, Quantity = quantity });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock for product ID: {ProductId}", productId);
                throw;
            }
        }
    }
} 
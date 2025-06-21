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
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(IConfiguration configuration, ILogger<OrderRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> CreateOrderAsync(Order order)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                const string sql = @"
                    INSERT INTO Orders (Id, CustomerId, ProductId, Quantity, OrderDate, Express) 
                    VALUES (@Id, @CustomerId, @ProductId, @Quantity, @OrderDate, @Express)";

                var parameters = new
                {
                    order.Id,
                    order.CustomerId,
                    order.ProductId,
                    order.Quantity,
                    order.OrderDate,
                    order.Express
                };

                var rowsAffected = await connection.ExecuteAsync(sql, parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for CustomerId: {CustomerId}, ProductId: {ProductId}", 
                    order.CustomerId, order.ProductId);
                throw;
            }
        }
    }
} 
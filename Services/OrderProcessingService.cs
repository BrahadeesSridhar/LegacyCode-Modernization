using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using LegacyCodeModernization.Models;
using LegacyCodeModernization.Interfaces;

namespace LegacyCodeModernization.Services
{
    public class OrderProcessingService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<OrderProcessingService> _logger;

        public OrderProcessingService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            ILogger<OrderProcessingService> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(bool Success, string Message)> ProcessOrderAsync(Guid customerId, Guid productId, int quantity, bool express)
        {
            try
            {
                // Input validation
                if (customerId == Guid.Empty)
                    return (false, "Invalid customer ID");
                
                if (productId == Guid.Empty)
                    return (false, "Invalid product ID");
                
                if (quantity <= 0)
                    return (false, "Quantity must be greater than zero");

                // Check product existence and stock
                var product = await _productRepository.GetProductAsync(productId);
                if (product == null)
                    return (false, "Product not found");

                if (product.Stock < quantity)
                    return (false, "Insufficient stock");

                // Update stock
                var stockUpdated = await _productRepository.UpdateStockAsync(productId, quantity);
                if (!stockUpdated)
                    return (false, "Failed to update stock");

                // Create order
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    ProductId = productId,
                    Quantity = quantity,
                    OrderDate = DateTime.UtcNow,
                    Express = express
                };

                var orderCreated = await _orderRepository.CreateOrderAsync(order);
                if (!orderCreated)
                    return (false, "Failed to create order");

                _logger.LogInformation(
                    "Order processed successfully. OrderId: {OrderId}, CustomerId: {CustomerId}, ProductId: {ProductId}, Quantity: {Quantity}, Express: {Express}",
                    order.Id, customerId, productId, quantity, express);

                return (true, express ? "Order processed with express delivery" : "Order processed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order for CustomerId: {CustomerId}, ProductId: {ProductId}", 
                    customerId, productId);
                return (false, "An unexpected error occurred while processing the order");
            }
        }
    }
} 
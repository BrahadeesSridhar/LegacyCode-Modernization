using Microsoft.AspNetCore.Mvc;
using LegacyCodeModernization.Services;
using LegacyCodeModernization.Models;

namespace LegacyCodeModernization.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderProcessingService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(OrderProcessingService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Process a new order
        /// </summary>
        /// <param name="request">Order details</param>
        /// <returns>Result of order processing</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ProcessOrder([FromBody] OrderRequest request)
        {
            try
            {
                var result = await _orderService.ProcessOrderAsync(
                    request.CustomerId,
                    request.ProductId,
                    request.Quantity,
                    request.Express);

                return Ok(new ApiResponse 
                { 
                    Success = result.Success,
                    Message = result.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order");
                return BadRequest(new ApiResponse 
                { 
                    Success = false,
                    Message = "Error processing order: " + ex.Message 
                });
            }
        }
    }

    public class OrderRequest
    {
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public bool Express { get; set; }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
} 
using System;
using System.Threading.Tasks;
using LegacyCodeModernization.Models;

namespace LegacyCodeModernization.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetProductAsync(Guid productId);
        Task<bool> UpdateStockAsync(Guid productId, int quantity);
    }
} 
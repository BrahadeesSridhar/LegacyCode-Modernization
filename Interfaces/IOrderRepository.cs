using System;
using System.Threading.Tasks;
using LegacyCodeModernization.Models;

namespace LegacyCodeModernization.Interfaces
{
    public interface IOrderRepository
    {
        Task<bool> CreateOrderAsync(Order order);
    }
} 
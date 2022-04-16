using Core.Models;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IOrderRepository
    {
        Task<ServiceResponse<bool>> PlaceOrder(string userId);
        //Task<ServiceResponseList<IReadOnlyList<Order>>> GetOrdersAsync(OrderSpecificationParams orderParams);
        Task<List<Order>> GetOrdersAsync(OrderSpecificationParams orderParams);
        Task<List<Order>> GetAllWithSpec(ISpecification<Order> spec);
        Task<Order> GetByIdWithSpec(ISpecification<Order> spec);
    }
}

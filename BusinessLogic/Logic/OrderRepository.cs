using BusinessLogic.Data;
using Core.Interfaces;
using Core.Models;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic
{
    public class OrderRepository : IOrderRepository
    {

        private readonly DataContext _context;
        private readonly ICartRepository _cartRepository;

        public OrderRepository(DataContext context, ICartRepository cartRepository)
        {
            _context = context;
            _cartRepository = cartRepository;
        }

        public async Task<List<Order>> GetOrdersAsync(OrderSpecificationParams orderParams)
        {
            var orders = await _context.Orders
                .Include( o => o.OrderItems)
                .ThenInclude(oi => oi.Product )
                .Where(
                    (o =>
                        (!orderParams.OrderDate.HasValue || o.OrderDate == orderParams.OrderDate))
                    )
                .OrderByDescending( o => o.OrderDate)
                .ToListAsync();


            return orders;
            //return new ServiceResponseList<IReadOnlyList<Order>> { Data = orders };
        }

        public async Task<List<Order>> GetAllWithSpec(ISpecification<Order> spec)
        {
            //var orders = await _context.Set<Order>().ToListAsync();
            var orders = await ApplySpecification(spec).ToListAsync();
            return orders;
            //return new ServiceResponseList<IReadOnlyList<Order>> { Data = orders };
        }

        public async Task<ServiceResponse<bool>> PlaceOrder(string userId)
        {
            var currentItemsInCart = await _cartRepository.GetDbCartProducts(userId);
            decimal totalPrice = 0;
            currentItemsInCart.ForEach(product => totalPrice += product.Price * product.Quantity);

            var orderItems = new List<OrderItem>();
            currentItemsInCart.ForEach(product => orderItems.Add(new OrderItem
            {
                ProductId = product.ProductId,
                ProductTypeId = product.ProductTypeId,
                Quantity = product.Quantity,
                TotalPrice = product.Price * product.Quantity
            }));

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalPrice = totalPrice,
                OrderItems = orderItems
            };

            _context.Orders.Add(order);

            _context.CartItems.RemoveRange(_context.CartItems
                .Where(ci => ci.UserId == userId));

            await _context.SaveChangesAsync();

            _context.CartItems.RemoveRange(_context.CartItems
             .Where(ci => ci.UserId == userId));

            return new ServiceResponse<bool> { Data = true };
        }


        private IQueryable<Order> ApplySpecification(ISpecification<Order> spec)
        {
            return SpecificationEvaluator<Order>.GetQuery(_context.Set<Order>().AsQueryable(), spec);
        }

        public async Task<Order> GetByIdWithSpec(ISpecification<Order> spec)
        {
            var order = await ApplySpecification(spec).FirstOrDefaultAsync();
            return order;
        }
    }
}

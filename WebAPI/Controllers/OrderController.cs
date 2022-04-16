using AutoMapper;
using BusinessLogic.Logic;
using Core.Interfaces;
using Core.Models;
using Core.Specifications;
using Core.Specifications.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTO.Order;
using WebAPI.Errors;
using WebAPI.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : BaseAPIController
    {

        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public OrderController(IOrderRepository orderRepository, UserManager<User> userManager, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _mapper = mapper;
        }



        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<ActionResult<ServiceResponseList<List<OrderOverviewReadDTO>>>> GetOrders([FromQuery] OrderSpecificationParams orderParams)
        {
            //var user = await _userManager.SearchUserAsync(HttpContext.User);
            var specs = new OrderSpecification(orderParams);
            var orders =  await _orderRepository.GetOrdersAsync(orderParams);

            var orderResponse = new List<OrderOverviewReadDTO>();
            orders.ForEach(o => orderResponse.Add(new OrderOverviewReadDTO
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                Product = o.OrderItems.Count > 1 ?
                $"{o.OrderItems.First().Product.Name } and" + $"{o.OrderItems.Count - 1} more..." :
                o.OrderItems.First().Product.Name,
                ProductImageURL = o.OrderItems.First().Product.ImageUrl,

            }
            ));

            var data = _mapper.Map<List<Order>, List<OrderOverviewReadDTO>>(orders);
            return Ok(new ServiceResponseList<List<OrderOverviewReadDTO>> { Data = orderResponse });
        }

        // GET: api/<ValuesController>
        [HttpGet("spec")]
        public async Task<ActionResult<ServiceResponseList<List<OrderOverviewReadDTO>>>> GetOrders2([FromQuery] OrderSpecificationParams orderParams)
        {
            var specs = new OrderSpecification(orderParams);
            var orders = await _orderRepository.GetAllWithSpec(specs);
            var orderResponse = new List<OrderOverviewReadDTO>();
            orders.ForEach(o => orderResponse.Add(new OrderOverviewReadDTO
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                Product = o.OrderItems.Count > 1 ?
                $"{o.OrderItems.First().Product.Name } and" + $"{o.OrderItems.Count - 1} more..." :
                o.OrderItems.First().Product.Name,
                ProductImageURL = o.OrderItems.First().Product.ImageUrl,

            }
            ));

            var data = _mapper.Map<List<Order>, List<OrderOverviewReadDTO>>(orders);
            return Ok(new ServiceResponseList<List<OrderOverviewReadDTO>> { Data = orderResponse });
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<OrderDetailsReadDTO>>> GetProductById(int id)
        {
            var specs = new OrderSpecification(id);
            var order = await _orderRepository.GetByIdWithSpec(specs);
            if (order == null) return NotFound(new CodeErrorResponse(404));

            var response = new ServiceResponse<OrderDetailsReadDTO>();

            var orderDetailsResponse = new OrderDetailsReadDTO
            {
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Products = new List<OrderDetailsProductDTO>()
            };

            order.OrderItems.ForEach(item =>
            orderDetailsResponse.Products.Add(new OrderDetailsProductDTO
            {
                ProductId = item.ProductId,
                ImageUrl = item.Product.ImageUrl,
                ProductType = item.ProductType.Name,
                Quantity = item.Quantity,
                Title = item.Product.Name,
                TotalPrice = item.TotalPrice
            }));

            response.Data = orderDetailsResponse;
            return Ok(response);
            return Ok(
                   new ServiceResponse<OrderDetailsReadDTO>
                   {
                       Data = _mapper.Map<Order, OrderDetailsReadDTO>(order)
                   }
               );
        }

        // POST api/<ValuesController>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<bool>>> PlaceOrder()
        {
            var user = await _userManager.SearchUserAsync(HttpContext.User);
            var result = await _orderRepository.PlaceOrder(user.Id);
            return Ok(result);
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

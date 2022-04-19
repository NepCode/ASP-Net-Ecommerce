using AutoMapper;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Extensions;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public PaymentController(IOrderRepository orderRepository, IPaymentRepository paymentRepository, UserManager<User> userManager, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _userManager = userManager;
            _mapper = mapper;
        }





        // POST api/<ValuesController>
        [Authorize]
        [HttpPost("checkout/order/{orderId}")]
        public async Task<ActionResult<ServiceResponse<string>>> StripeCreateCheckoutSession(int orderId)
        {
            var user = await _userManager.SearchUserAsync(HttpContext.User);
            var session = await _paymentRepository.StripeCreateCheckoutSession(user, orderId);
            return Ok(new ServiceResponse<string> { Data = session.Url });
        }
    }


}

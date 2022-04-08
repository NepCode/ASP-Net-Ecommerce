using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartService;
        private readonly UserManager<User> _userManager;

        public CartController(ICartRepository cartService, UserManager<User> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }


        // GET: api/<CartController>/products
        [HttpPost("products")]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> GetCartProducts(List<CartItem> cartItems)
        {
            var result = await _cartService.GetCartProducts(cartItems);
            return Ok(new ServiceResponse<IReadOnlyList<CartProductResponse>>
            {
                Data = result,
            });
        }

        // GET api/<CartController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<CartController>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> StoreCartItems(List<CartItem> cartItems)
        {
            var user = await _userManager.SearchUserAsync(HttpContext.User);
            //var userrr4 = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _cartService.StoreCartItems(cartItems, user );
            return Ok(result);
        }

        // PUT api/<CartController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CartController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

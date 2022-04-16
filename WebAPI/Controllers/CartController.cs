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
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<User> _userManager;

        public CartController(ICartRepository cartService, UserManager<User> userManager)
        {
            _cartRepository = cartService;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<ActionResult<ServiceResponseList<List<CartProductResponse>>>> AddToCart(CartItem cartItem)
        {
            var user = await _userManager.SearchUserAsync(HttpContext.User);
            var result = await _cartRepository.AddToCart(cartItem, user.Id);
            return Ok(result);
        }


        // GET: api/<CartController>/products
        [HttpPost("products")]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> GetCartProducts(List<CartItem> cartItems)
        {
            var result = await _cartRepository.GetCartProducts(cartItems);
            return Ok(new ServiceResponse<List<CartProductResponse>>
            {
                Data = result,
            });
        }

        [Authorize]
        [HttpGet("count")]
        public async Task<ActionResult<ServiceResponse<int>>> GetCartItemsCount()
        {
            var user = await _userManager.SearchUserAsync(HttpContext.User);
            return await _cartRepository.GetCartItemsCount(user);
        }

        // POST api/<CartController>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ServiceResponseList<List<CartProductResponse>>>> StoreCartItems(List<CartItem> cartItems)
        {
            var user = await _userManager.SearchUserAsync(HttpContext.User);
            //var userrr4 = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _cartRepository.StoreCartItems(cartItems, user );
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> GetDbCartProducts()
        {
            var user = await _userManager.SearchUserAsync(HttpContext.User);
            return Ok(new ServiceResponse<IReadOnlyList<CartProductResponse>>
            {
                Data = await _cartRepository.GetDbCartProducts(user.Id)
            });
        }

        // PUT api/<CartController>/5
        [Authorize]
        [HttpPut("quantity")]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdateQuantity (CartItem cartItem)
        {
            var user = await _userManager.SearchUserAsync(HttpContext.User);
            var result = await _cartRepository.UpdateQuantity(cartItem, user.Id);
            return Ok(result);

        }

        // DELETE api/<CartController>/5
        [Authorize]
        [HttpDelete("{productId}/{productTypeId}")]
        public async Task<ActionResult<ServiceResponse<bool>>> RemoveItemFromCart( int productId, int productTypeId )
        {
            var user = await _userManager.SearchUserAsync(HttpContext.User);
            var result = await _cartRepository.RemoveItemFromCart( productId, productTypeId, user.Id);
            return Ok(result);

        }
    }
}

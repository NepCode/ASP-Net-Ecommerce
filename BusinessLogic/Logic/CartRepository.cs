using BusinessLogic.Data;
using Core.Interfaces;
using Core.Models;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic
{
    public class CartRepository : ICartRepository
    {
        private readonly DataContext _context;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly UserManager<User> _userManager;

        public CartRepository(DataContext context, IGenericRepository<Product> productRepository, UserManager<User> userManager)
        {
            _context = context;
            _productRepository = productRepository;
            _userManager = userManager;
        }
        public async Task<ServiceResponse<bool>> AddToCart(CartItem cartItem, string userId  )
        {
            var sameItem = await _context.CartItems
                .FirstOrDefaultAsync( ci => ci.ProductId == cartItem.ProductId &&
                ci.ProductTypeId == cartItem.ProductTypeId &&
                ci.UserId == userId );
            if(sameItem == null)
            {
                _context.CartItems.Add( cartItem );
            }
            else
            {
                sameItem.Quantity += cartItem.Quantity ;
            }
            await _context.SaveChangesAsync();
            return new ServiceResponse<bool> { Data = true };
        }

        public async Task<ServiceResponse<int>> GetCartItemsCount(User user)
        {
            var count = (await _context.CartItems.Where(ci => ci.UserId == user.Id ).ToListAsync()).Count;
            return new ServiceResponse<int> { Data = count };
        }

        public async Task<List<CartProductResponse>> GetCartProducts(List<CartItem> cartItems)
        {
            var result = new List<CartProductResponse>();
           

            foreach (var item in cartItems)
            {
                var specs = new ProductWithCategoryAndBrandSpecification(item.ProductId);
                var product = await _productRepository.GetByIdWithSpec(specs);

               if (product == null)
                {
                    continue;
                }

                var productVariant = await _context.ProductVariants
                    .Where(v => v.ProductId == item.ProductId && v.ProductTypeId == item.ProductTypeId)
                    .Include(v => v.ProductType)
                    .FirstOrDefaultAsync();

                if (productVariant == null)
                {
                    continue;
                }

                var cartProduct = new CartProductResponse
                {
                    ProductId = product.Id,
                    Title = product.Name,
                    ImageURL = product.ImageUrl,
                    Price = productVariant.Price,
                    ProductType = productVariant.ProductType.Name,
                    ProductTypeId = productVariant.ProductTypeId,
                    Quantity = item.Quantity,
                };

                result.Add(cartProduct);
            }

            return result;
        }

        public async Task<List<CartProductResponse>> GetDbCartProducts(string? userId = null)
        {
            return await GetCartProducts(await _context.CartItems
                .Where(ci => ci.UserId == userId).ToListAsync());
        }

        public async Task<ServiceResponse<bool>> RemoveItemFromCart(int productId, int productTypeId, string userId)
        {
            var dbCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == productId &&
                ci.ProductTypeId == productTypeId &&
                ci.UserId == userId);
            if (dbCartItem == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Cart item does not exist."
                };
            }

            _context.CartItems.Remove(dbCartItem);
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true };
        }

        public async Task<List<CartProductResponse>> StoreCartItems(List<CartItem> cartItems, User user)
        {
            //var userdd = await _userManager.SearchUserAsync(HttpContext.User);
            cartItems.ForEach(cartItem => cartItem.UserId = user.Id );
            _context.CartItems.AddRange(cartItems);
            await _context.SaveChangesAsync();

            /*return await GetCartProducts(await _context.CartItems
                .Where(ci => ci.UserId == user.Id).ToListAsync());*/
            return await GetDbCartProducts(user.Id);
        }

        public async Task<ServiceResponse<bool>> UpdateQuantity(CartItem cartItem, string userId)
        {
            var dbCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == cartItem.ProductId &&
                ci.ProductTypeId == cartItem.ProductTypeId &&
                ci.UserId == userId);
            if (dbCartItem == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Success = false,
                    Message = "Cart item does not exist."
                };
            }

            dbCartItem.Quantity = cartItem.Quantity;
            await _context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true };
        }
    }
}

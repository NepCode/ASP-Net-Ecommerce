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
        public Task<bool> AddToCart(CartItem cartItem)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCartItemsCount()
        {
            throw new NotImplementedException();
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

        public Task<List<CartProductResponse>> GetDbCartProducts(int? userId = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveItemFromCart(int productId, int productTypeId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<CartProductResponse>> StoreCartItems(List<CartItem> cartItems, User user)
        {
            var userdd = await _userManager.SearchUserAsync(HttpContext.User);
            cartItems.ForEach(cartItem => cartItem.UserId = user.Id );
            _context.CartItems.AddRange(cartItems);
            await _context.SaveChangesAsync();

            return await GetCartProducts(await _context.CartItems
                .Where(ci => ci.UserId == user.Id).ToListAsync());
        }

        public Task<bool> UpdateQuantity(CartItem cartItem)
        {
            throw new NotImplementedException();
        }
    }
}

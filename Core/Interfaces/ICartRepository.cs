using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICartRepository
    {
        Task<List<CartProductResponse>> GetCartProducts(List<CartItem> cartItems);
        Task<List<CartProductResponse>> StoreCartItems(List<CartItem> cartItems, User user);
        Task<ServiceResponse<int>> GetCartItemsCount(User user);
        Task<List<CartProductResponse>>GetDbCartProducts(string? userId = null);
        Task<ServiceResponse<bool>> AddToCart(CartItem cartItem, string userId  );
        Task<ServiceResponse<bool>> UpdateQuantity(CartItem cartItem, string userId);
        Task<ServiceResponse<bool>> RemoveItemFromCart(int productId, int productTypeId, string userId);
    }
}

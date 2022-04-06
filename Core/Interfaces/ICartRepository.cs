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
        Task<List<CartProductResponse>> StoreCartItems(List<CartItem> cartItems);
        Task<int> GetCartItemsCount();
        Task<List<CartProductResponse>>GetDbCartProducts(int? userId = null);
        Task<bool>AddToCart(CartItem cartItem);
        Task<bool> UpdateQuantity(CartItem cartItem);
        Task<bool> RemoveItemFromCart(int productId, int productTypeId);
    }
}

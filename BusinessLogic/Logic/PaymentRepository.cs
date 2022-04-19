using BusinessLogic.Data;
using Core.Interfaces;
using Core.Models;
using Core.Specifications.Order;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DataContext _context;
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;

        public PaymentRepository(DataContext context, ICartRepository cartRepository, IOrderRepository orderRepository)
        {
            StripeConfiguration.ApiKey = "sk_test_51KpveSBzPaJPqRetUl5Dr01cDLZvhBpqheiJOZqQvoxnJ2XXzEQHqepNrPhebli2t9gK38UHGK05pQPlUaXqzwyj00ADZDdrfD";
            _context = context;
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
        }

        public async Task<Session> StripeCreateCheckoutSession(User user, int orderId)
        {
            //var products = (await _cartRepository.GetDbCartProducts("asd"));
            var specs = new OrderSpecification(orderId);
            var order = await _orderRepository.GetByIdWithSpec(specs);

            var lineItems = new List<SessionLineItemOptions>();
            //var Products = new List<OrderDetailsProductDTO>();

            /*
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
            */

            order.OrderItems.ForEach(product => lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = product.TotalPrice * 100,
                    Currency = "mxn",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = product.Product.Name,
                        //Images = new List<string> { product.Product.ImageUrl }
                    }
                },
                Quantity = product.Quantity,
            }));


            var options = new SessionCreateOptions
            {
                CustomerEmail = user.Email,
                ShippingAddressCollection =
                    new SessionShippingAddressCollectionOptions
                    {
                        AllowedCountries = new List<string> { "MX" }
                    },
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:7093/order-success",
                CancelUrl = "https://localhost:7093/cart"
            };

            var service = new SessionService();
            Session session = service.Create(options);
            return session;

        }
    }
}

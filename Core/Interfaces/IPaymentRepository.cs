using Core.Models;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Session> StripeCreateCheckoutSession(User user,int orderId);
        //Task<ServiceResponse<bool>> StripeFulfillOrder(HttpRequest request);
    }
}

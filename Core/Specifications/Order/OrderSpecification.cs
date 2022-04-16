using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications.Order
{
    public class OrderSpecification : BaseSpecification<Models.Order>
    {

        public OrderSpecification(OrderSpecificationParams orderParams) : base
           (x =>
                (!orderParams.OrderDate.HasValue || x.OrderDate == orderParams.OrderDate) 
           )

        {
            AddInclude(o => o.OrderItems);
            //AddInclude("OrderItems.Product"); // string syntax works for any level of depth
            AddInclude($"{nameof(Models.Order.OrderItems)}.{nameof(OrderItem.Product)}");

            ApplyPaging(orderParams.PageSize * (orderParams.PageIndex - 1), orderParams.PageSize);

            if (!string.IsNullOrEmpty(orderParams.Sort))
            {
                switch (orderParams.Sort)
                {
                    case "idAsc":
                        AddOrder(o => o.Id);
                        break;
                    case "idDesc":
                        AddOrderByDesc(o => o.Id);
                        break;
                    case "priceAsc":
                        AddOrder(o => o.TotalPrice);
                        break;
                    case "priceDesc":
                        AddOrderByDesc(o => o.TotalPrice);
                        break;
                    default:
                        AddOrder(o => o.Id);
                        break;
                }
            }


        }



        public OrderSpecification(int id) : base (x => x.Id == id )
        {
            AddInclude(o => o.OrderItems);
            AddInclude($"{nameof(Models.Order.OrderItems)}.{nameof(OrderItem.Product)}");
            AddInclude($"{nameof(Models.Order.OrderItems)}.{nameof(OrderItem.ProductType)}");

        }



    }

}

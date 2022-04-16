using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class OrderItem
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

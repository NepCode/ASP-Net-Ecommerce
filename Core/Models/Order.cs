using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Order : BaseClass
    {
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;

        public decimal TotalPrice { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}

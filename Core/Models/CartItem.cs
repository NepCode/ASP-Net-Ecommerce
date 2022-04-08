using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class CartItem
    {
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int ProductTypeId { get; set; }
        public int Quantity { get; set; }
    }
}

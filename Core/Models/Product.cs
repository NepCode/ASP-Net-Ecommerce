using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Product : BaseClass
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        /* public decimal Price { get; set; } */
        public bool Featured  { get; set; } = false;
        public List<ProductVariant> Variants   { get; set; } = new List<ProductVariant>();
    }
}

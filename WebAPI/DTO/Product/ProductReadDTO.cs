using Core.Models;

namespace WebAPI.DTO.Product
{
    public class ProductReadDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        /* public decimal Price { get; set; } */
        public bool Featured  { get; set; } = false;
        public List<ProductVariant> Variants   { get; set; } = new List<ProductVariant>();
    }
}

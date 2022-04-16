using Core.Models;

namespace WebAPI.DTO.Order
{
    public class OrderOverviewReadDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalPrice { get; set; }
        public string Product { get; set; } 
        public string ProductImageURL { get; set; }
    }
}

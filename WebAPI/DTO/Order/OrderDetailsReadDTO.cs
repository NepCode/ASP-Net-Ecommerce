namespace WebAPI.DTO.Order
{
    public class OrderDetailsReadDTO
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderDetailsProductDTO> Products { get; set; }
    }
}

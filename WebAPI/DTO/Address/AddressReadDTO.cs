namespace WebAPI.DTO.Address
{
    public class AddressReadDTO
    {
        public int Id { set; get; }
        public string Street { set; get; }
        public string City { set; get; }
        public string Apartment { set; get; }
        public string PostalCode { set; get; }
        public string Country { set; get; }
    }
}

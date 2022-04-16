using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Address : BaseClass
    {
        public string Street { set; get; }
        public string City { set; get; }
        public string Apartment { set; get; }
        public string PostalCode { set; get; }
        public string Country { set; get; }
        public string UserId { set; get; }
        public User User { set; get; }
    }
}

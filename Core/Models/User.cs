using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class User: IdentityUser
    {
        public string Name { set; get; }
        public string LastName { set; get; }
        public List<Address> Addresses { set; get; }
    }
}

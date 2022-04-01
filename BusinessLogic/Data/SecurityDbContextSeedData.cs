using Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Data
{
    public class SecurityDbContextSeedData
    {

        public static async Task SeedUserAsync(UserManager<User> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new User
                {
                    Name = "neptune",
                    LastName = "neptune",
                    Email = "neptune@gmail.com",
                    UserName = "neptune",
                    /*Address = new Address
                    {
                        Street = "mikyway 658",
                        City = "neptune",
                        Apartment = "space",
                        PostalCode = "387482"
                    }*/
                    Addresses = new List<Address>
                        {
                            new Address {
                            Street = "milkyway 658",
                            City = "neptune",
                            Apartment = "space",
                            PostalCode = "387482",
                            Country = "MX"
                            },

                            new Address {
                            Street = "rings 667",
                            City = "saturn",
                            Apartment = "space",
                            PostalCode = "681396844",
                            Country = "MX"
                            }
                        }
                };
                await userManager.CreateAsync(user, "!Milkyway539");
            }

            /*if (!roleManager.Roles.Any())
            {
                var role = new IdentityRole
                {
                    Name = "ADMIN"
                };
                await roleManager.CreateAsync(role);
            }*/

        }


    }
}

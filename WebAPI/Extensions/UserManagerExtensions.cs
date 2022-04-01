using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebAPI.Extensions
{
    public static class UserManagerExtensions
    {

        public static async Task<User> SearchUserAsync(this UserManager<User> input, ClaimsPrincipal usr)
        {
            var email = usr?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var user = await input.Users.SingleOrDefaultAsync(x => x.Email == email);

            return user;
        }

        public static async Task<User> SearchUserWithAddressAsync(this UserManager<User> input, ClaimsPrincipal usr)
        {
            var email = usr?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var user = await input.Users.Include(x => x.Addresses).SingleOrDefaultAsync(x => x.Email == email);

            return user;
        }


    }
}

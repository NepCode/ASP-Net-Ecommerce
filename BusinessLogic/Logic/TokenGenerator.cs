using Core.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic
{
    public class TokenGenerator : ITokenGenerator
    {
        public string Generate(string secretKey, string issuer, string audience, string expires, IList<Claim> claims = null)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            JwtSecurityToken securityToken = new(
                issuer, 
                audience,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(double.Parse(expires)),
                credentials);
            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}

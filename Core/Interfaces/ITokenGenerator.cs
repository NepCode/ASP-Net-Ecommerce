using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ITokenGenerator
    {
        /// <summary>
        /// Generates jwt token.
        /// </summary>
        /// <param name="secretKey">The secret key for security.</param>
        /// <param name="issuer">The issuer.</param>
        /// <param name="audience">The audience.</param>
        /// <param name="expires">The expire time.</param>
        /// <param name="claims"><see cref="IEnumerable{T}"/></param>
        /// <returns>Generated token.</returns>
        string Generate(string secretKey, string issuer, string audience, string expires, IList<Claim> claims = null);
    }
}

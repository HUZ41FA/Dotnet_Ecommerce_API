using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Utilities.Helper
{
    public static class Helper
    {
        public static bool ConfirmPassword(string password, string confirmPassword)
        {
            return password == confirmPassword;
        }

        public static JwtSecurityToken GenerateJwtToken(List<Claim> authClaims, JwtConfig configuration)
        {
            SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.Secret));

            JwtSecurityToken token = new JwtSecurityToken(
                    issuer: configuration.ValidHost,
                    audience: configuration.ValidAudience,
                    expires: DateTime.Now.AddMinutes(60),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return token;
        }
    }
}

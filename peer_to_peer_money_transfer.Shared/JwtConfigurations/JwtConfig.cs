using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using peer_to_peer_money_transfer.DAL.Entities;
using peer_to_peer_money_transfer.Shared.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace peer_to_peer_money_transfer.Shared.JwtConfigurations
{
    public class JwtConfig : IJwtConfig
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public JwtConfig(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> GenerateJwtToken(ApplicationUser user)
        { 
            var signInCredentials = GetSignInCredentials();
            var claims = await GetClaims(user);
            var jwtToken = GenerateToken(signInCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        private SigningCredentials GetSignInCredentials()
        {
            //var Key = Environment.GetEnvironmentVariable("Key");
            var Key = _configuration.GetSection("Jwt:Key").Value;
            var encodeKey = Encoding.UTF8.GetBytes(Key);
            var signInCredential = new SymmetricSecurityKey(encodeKey);

            return new SigningCredentials(signInCredential, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(ApplicationUser user)
        {
            // Create some claims for the token
            var claims = new List<Claim>
            { 
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),
            };

            //Getting claims assigned to user
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            //Get user roles and add to claims
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            return claims;
        }

        private JwtSecurityToken GenerateToken(SigningCredentials signInCredentials, List<Claim> claims)
        {
            /*var issuer = Environment.GetEnvironmentVariable("Issuer");
            var lifetime = Environment.GetEnvironmentVariable("Lifetime");*/
            var issuer = _configuration.GetSection("Jwt:Issuer").Value;
            var audience = _configuration.GetSection("Jwt:Audience").Value;
            var lifetime = _configuration.GetSection("Jwt:Lifetime").Value;
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(lifetime));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: signInCredentials
            );
            return token;
        }
    }
}

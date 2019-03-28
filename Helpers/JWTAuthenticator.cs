using System;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using CounterCulture.Repositories.Models;

namespace CounterCulture.Auth.Helpers {

    public static class JWTAuthenticator {

        public static AuthResponse Authenticate(User user, string secret){
            if(user == null) return null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tomorrow = DateTime.UtcNow.AddDays(1);
            var descriptor = new SecurityTokenDescriptor(){
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim("UserId", user.ID.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = tomorrow,
                SigningCredentials = new SigningCredentials(
                                     new SymmetricSecurityKey(key), 
                                     SecurityAlgorithms.HmacSha256Signature)
            };
            var _token = tokenHandler.CreateToken(descriptor);
            var token = tokenHandler.WriteToken(_token);
            var expiresIn = (tomorrow - DateTime.UtcNow);
            return (new AuthResponse(){
                access_token = token,
                token_type = "bearer",
                expires_in = Math.Floor(expiresIn.TotalSeconds),
                expiration_date = descriptor.Expires
            });
        }
        
    }

}
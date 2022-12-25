using AuthenticationAndAuthorizationWithJWT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationAndAuthorizationWithJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;

        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            var user = Authenticate(userLogin);

            if (user != null)
            {
                var token = Generate(user);
                return Ok(token);
            }

            return NotFound("User not found");
        }

        private string Generate(UserModel user)
        {
            // Get security key from config file
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Claims are used to store the data about the user.
            
            // What are claims?
            // Claims are statements about an entity (typically, the user) and additional metadata.
            // For example, a claim can be used to represent the username of a logged-in user.
            // Claims can be used to represent any information in a name/value pair.
            // The name is a claim type, and the value is the claim value.
            // The claim type is a string that identifies the claim.
            // The claim value is a string or object that contains the value of the claim.
 
            // In simple words, claims are the information about the user that we want to store in the token.
            // We can store any information in the claims.
            // We can store the user id, username, email, role, etc. in the claims.
            // We can also store the user's permissions in the claims.

            // Claims are the properties describing the user that is authenticated by the token.
            // For instance, the user's name, email, role, etc. can be stored in the claims.
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.GivenName, user.GivenName),
                new Claim(ClaimTypes.Surname, user.Surname),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Create the token object
            // Who is the issuer, who is the audience.
            // It checks has the issuer, audience changed in the meantime from the time we have defined in the program class
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(15),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserModel Authenticate(UserLogin userLogin)
        {
            var currentUser = UserConstants.Users.FirstOrDefault(o => o.Username.ToLower() == userLogin.Username.ToLower() && o.Password == userLogin.Password);

            if (currentUser != null)
            {
                return currentUser;
            }

            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AuthenticationAndAuthorizationWithJWT.Dto;
using AuthenticationAndAuthorizationWithJWT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationAndAuthorizationWithJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        /*
         * https://www.youtube.com/watch?v=v7q3pEK1EA0&ab_channel=PatrickGod
         * 
         */
        
        public static User user = new User();
        private readonly IConfiguration _configuration;
        // private readonly IUserService _userService;

        public AuthController(IConfiguration configuration)//, IUserService userService)
        {
            _configuration = configuration;
            //_userService = userService;
        }

        // [HttpGet, Authorize]
        // public ActionResult<string> GetMe()
        // {
        //     var userName = _userService.GetMyName();
        //     return Ok(userName);
        // }
        [HttpGet("TestEndPoint")]
        public async Task<ActionResult<User>> TestEndPoint()
        {
            
            User user = new User();
            // user.Username = request.Username;
            // user.PasswordHash = passwordHash;
            // user.PasswordSalt = passwordSalt;

            return Ok(user);
        }
        
        [HttpGet("GetMe"), Authorize]
        public ActionResult<string> GetMe()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            
            User user = GetCurrentUser();
            return Ok(userName);
        }
        
        // Get the info about the user
        private User GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            
            var userRole = User.FindFirstValue(ClaimTypes.Role);
   
            if (identity != null)
            {
                // It is IEnumerable i.e. a list
                var userClaims = identity.Claims;

                return new User
                {
                    Username = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                    // Role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value,
                    
                    //Username = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    //GivenName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.GivenName)?.Value,
                    //Surname = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Surname)?.Value,
                };
            }
            return null;
        }

        // Create the password salt and password hash while registering the user.
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserSignupDto request)
        {
            // If user already exists, return bad request
            // _context.Users.Any(x => x.Email == request.Email)
            // if (request.Username != null)
            // {
            //     return BadRequest("User already exists");
            // }
            
            if(IsValidEmail(request.Email) == false)
            {
                return BadRequest("Invalid Email");
            }
            
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // return Ok(user);
            // Shows 201 StatusCode 
            return Created("~/api/auth", user); // new {id = user.Id});
        }
        
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                // This Key is the instance of hmac and is used as our salt
                passwordSalt = hmac.Key;
                // Get the password hash using the salt obtained by password
                // Password salt is used to generate the hash
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                    RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
    

        // During login, we will again create the password hash and compare it with the password salt stored in the database.
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto request)
        {
            if(IsValidEmail(request.Email) == false)
            {
                return BadRequest("Invalid Email");
            }
            
            if (user.Username != request.Username)
            {
                return BadRequest("User not found.");
            }
            
            // Find user in database
            // var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == request.Email);
            // if (user == null)
            // {
            //     return BadRequest("User not found.");
            // }

            // We will compare the password entered by the user while logging in with the password hash and salt
            // of the user that is stored in the database.
            // Verify the password entered by the user with the password hash and password salt stored in the DB
            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password.");
            }

            string token = CreateToken(user);

            // var refreshToken = GenerateRefreshToken();
            // SetRefreshToken(refreshToken);

            return Ok(token);
        }

        // We will compare the password entered by the user while logging in with the password hash and salt
        // of the user that is stored in the database.
        // Verify the password hash and password salt with the input password
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                // Create the password hash using the password salt
                // We got the password the user entered while logging in and encrypted it using the password salt
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                
                // Compare the computed password hash with the password hash stored in the database
                // We will compare the resulting computed hash generated by us with the stored password hash from the database
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        
        private string CreateToken(User user)
        {
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
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "Admin"),
                // Create a claim for the user id
                // new Claim(ClaimTypes.NameIdentifier, "1"),
                // new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"]),
                // new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"])
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Key").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Token will expire in 1 day
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        // [HttpPost("refresh-token")]
        // public async Task<ActionResult<string>> RefreshToken()
        // {
        //     var refreshToken = Request.Cookies["refreshToken"];
        //
        //     if (!user.RefreshToken.Equals(refreshToken))
        //     {
        //         return Unauthorized("Invalid Refresh Token.");
        //     }
        //     else if(user.TokenExpires < DateTime.Now)
        //     {
        //         return Unauthorized("Token expired.");
        //     }
        //
        //     string token = CreateToken(user);
        //     var newRefreshToken = GenerateRefreshToken();
        //     SetRefreshToken(newRefreshToken);
        //
        //     return Ok(token);
        // }
        //
        // private RefreshToken GenerateRefreshToken()
        // {
        //     var refreshToken = new RefreshToken
        //     {
        //         Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
        //         Expires = DateTime.Now.AddDays(7),
        //         Created = DateTime.Now
        //     };
        //
        //     return refreshToken;
        // }
        //
        // private void SetRefreshToken(RefreshToken newRefreshToken)
        // {
        //     var cookieOptions = new CookieOptions
        //     {
        //         HttpOnly = true,
        //         Expires = newRefreshToken.Expires
        //     };
        //     Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
        //
        //     user.RefreshToken = newRefreshToken.Token;
        //     user.TokenCreated = newRefreshToken.Created;
        //     user.TokenExpires = newRefreshToken.Expires;
        // }
    }
}

using AuthenticationAndAuthorizationWithJWT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace AuthenticationAndAuthorizationWithJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public class Members
        {
            public int MemberId { get; set; }
            public string FirstName { get; set; }
            public string  LastName { get; set; }
            public string Address { get; set; }
        }
        

        
        [HttpGet("GetAllMember")]
        [Authorize]
        public List<Members> GetAllMember()
        {
            List<Members> lisMembers = new List<Members>
            {
                new Members{MemberId=1, FirstName="Kirtesh", LastName="Shah", Address="Vadodara" },
                new Members{MemberId=2, FirstName="Nitya", LastName="Shah", Address="Vadodara" },
                new Members{MemberId=3, FirstName="Dilip", LastName="Shah", Address="Vadodara" },
                new Members{MemberId=4, FirstName="Atul", LastName="Shah", Address="Vadodara" },
                new Members{MemberId=5, FirstName="Swati", LastName="Shah", Address="Vadodara" },
                new Members{MemberId=6, FirstName="Rashmi", LastName="Shah", Address="Vadodara" },
            };
            
            return lisMembers;
        }
        
        [HttpGet("Admins")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminsEndpoint()
        {
            var currentUser = GetCurrentUser();

            return Ok($"Hi {currentUser.GivenName}, you are an {currentUser.Role}");
        }


        [HttpGet("Sellers")]
        [Authorize(Roles = "Seller")]
        public IActionResult SellersEndpoint()
        {
            var currentUser = GetCurrentUser();

            return Ok($"Hi {currentUser.GivenName}, you are a {currentUser.Role}");
        }

        [HttpGet("AdminsAndSellers")]
        [Authorize(Roles = "Administrator,Seller")]
        public IActionResult AdminsAndSellersEndpoint()
        {
            var currentUser = GetCurrentUser();

            return Ok($"Hi {currentUser.GivenName}, you are an {currentUser.Role}");
        }

        [HttpGet("Public")]
        public IActionResult Public()
        {
            return Ok("Hi, you're on public property");
        }

        // Get the info about the user
        private UserModel GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                // It is IEnumerable i.e. a list
                var userClaims = identity.Claims;

                return new UserModel
                {
                    Username = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    EmailAddress = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                    GivenName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.GivenName)?.Value,
                    Surname = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Surname)?.Value,
                    Role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value
                };
            }
            return null;
        }
    }
}

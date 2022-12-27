using System.ComponentModel.DataAnnotations;

namespace AuthenticationAndAuthorizationWithJWT.Dto;

public class UserLoginDto
{
    public string Username { get; set; } = string.Empty;
    
    [Required, EmailAddress]
    [RegularExpression(@"^(?=.{1,254}$)(?=.{1,64}@)[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
    public string Email { get; set; } = string.Empty;
    
    [Required, MinLength(6, ErrorMessage = "Please enter a password with at least 6 characters")]
    public string Password { get; set; } = string.Empty;
}
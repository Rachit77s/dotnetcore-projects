using System.ComponentModel.DataAnnotations;

namespace AuthenticationAndAuthorizationWithJWT.Dto;

public class UserSignupDto
{    
     public string Username { get; set; } = string.Empty;
     
     [Required, EmailAddress]
     public string Email { get; set; } = string.Empty;
    
     [Required, MinLength(6, ErrorMessage = "Please enter a password with at least 6 characters")]
     public string Password { get; set; } = string.Empty;
    
     [Required, Compare("Password")]
     public string ConfirmPassword { get; set; } = string.Empty;

}
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Application.Dtos.Authentication
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(255)]
        public string Email { get; set; }
        [Required]
        [MaxLength(255)]
        public string Password { get; set; }
        [Required]
        [MaxLength(255)]
        public string ConfirmPassword { get; set; }
    }
}

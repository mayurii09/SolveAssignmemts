using System.ComponentModel.DataAnnotations;

namespace ProductManagementSystem.Models
{
    public class RegisterModel
    {
        [Required]
        public string? FullName { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }

    public class LoginModel
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}

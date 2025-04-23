using Microsoft.AspNetCore.Identity;

namespace ProductManagementSystem.Models
{
    public class Customer : IdentityUser
    {
        public string? FullName { get; set; }
    }
}

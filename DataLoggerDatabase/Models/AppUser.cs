using Microsoft.AspNetCore.Identity;

namespace DataLoggerDatabase.Models
{
    public class AppUser : IdentityUser<long>
    {
        public bool IsAdmin { get; set; }
    }
}

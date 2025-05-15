using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TruckDispatcherApi.Services
{
    public class ChangeRolesDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public List<IdentityRole> AllRoles { get; set; }

        [Required]
        public IList<string> UserRoles { get; set; }
    }
}

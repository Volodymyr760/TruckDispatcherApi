using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TruckDispatcherApi.Library;
using Microsoft.AspNetCore.Identity;

namespace TruckDispatcherApi.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "First name (1-20 characters) is required."), StringLength(20)]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last name (1-20 characters) is required."), StringLength(20)]
        public required string LastName { get; set; }

        [StringLength(50)]
        public string? Avatar { get; set; }

        #region Account

        [Required(ErrorMessage = "Company name (1-50 characters) is required."), StringLength(50)]
        public required string CompanyName { get; set; }

        [StringLength(20)]
        public string? MC { get; set; }

        [StringLength(20)]
        public string? DOT { get; set; }

        [Required]
        public AccountStatus AccountStatus { get; set; }

        [Required]
        [Column(TypeName = "datetime2(0)")]
        public DateTime StartPayedPeriodDate { get; set; }

        [Required]
        [Column(TypeName = "datetime2(0)")]
        public DateTime FinishPayedPeriodDate { get; set; }

        [Required(ErrorMessage = "Last login date at is required.")]
        [Column(TypeName = "datetime2(0)")]
        public DateTime LastLoginDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "datetime2(0)")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        #endregion Account

        #region DefaultSearchSettings

        public int SearchDeadheads { get; set; }

        public int SearchMilesMin { get; set; }

        public int SearchMilesMax { get; set; }

        [Required(ErrorMessage = "SearchSortField (1-20 characters) is required."), StringLength(20)]
        public required string SearchSortField { get; set; }

        [Required]
        public OrderType SearchSort { get; set; }

        #endregion DefaultSearchSettings


        public List<Driver> Drivers { get; set; } = [];

        public List<Load> Loads { get; set; } = [];

        public List<Truck> Trucks { get; set; } = [];
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Services
{
    public class UserDto
    {
        public string? Id { get; set; }

        public string? UserName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [DataType(DataType.Text)]
        [Display(Name = "FirstName")]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [DataType(DataType.Text)]
        [Display(Name = "LastName")]
        public required string LastName { get; set; }

        [StringLength(50)]
        public string? Avatar { get; set; }

        [Required(ErrorMessage = "Email (1-50 characters) is required."), StringLength(50)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public required string Email { get; set; }

        [Required]
        public bool EmailConfirmed { get; set; }

        [StringLength(20, ErrorMessage = "The {0} should not be bigger then {1} characters long.")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone")]
        public string? PhoneNumber { get; set; }

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


        public List<string> Tokens { get; set; } = [];

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

        public List<DriverDto> Drivers { get; set; } = [];

        public List<LoadDto> Loads { get; set; } = [];

        public List<TruckDto> Trucks { get; set; } = [];
    }
}

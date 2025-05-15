using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Models
{
    /// <summary>
    /// Heatmap  - функціонал, який показує в розрізі штатів "температуру" ринку перевезень.
    /// Heatmap об’єкт формується по даті ("Today" і "Tomorrow") і по типу Equipment
    /// Кожен об’єкт HeatmapItem в БД попередньо згенерований і містить поточні обчислені значення окремо для 
    /// "Today" і "Tomorrow" і для кожного виду Equipment, всього 49 штатів * 2 типи дня * 3 типи Equipment = 294 об’єкти
    /// Після імпорту ImportLoads адміністратор обнулює наявні значення в HeatmapItems і виконує генерацію актуальних значень
    /// для всіх об’єктів.
    /// Для кращої швидкодії в кеші зберігаються 6 об’єктів Heatmap з відповідними списками HeatmapItems:
    /// TodayFlatbed, TodayReefer, TodayVan, TomorrowFlatbed, TomorrowReefer, TomorrowVan.
    /// </summary>
    public class Heatmap
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string Id { get; set; }

        /// <summary>
        /// Допустимі значення "Today" і "Tomorrow", визначаються в залежності від DateTime.Now()
        /// </summary>
        [Required]
        [StringLength(50)]
        public required string DayType { get; set; }

        [Required]
        public Equipment Equipment { get; set; }

        public List<HeatmapState> HeatmapStates { get; set; } = [];

        [Required(ErrorMessage = "UpdatedAt is required")]
        [Column(TypeName = "datetime2(0)")]
        public DateTime UpdatedAt { get; set; }
    }
}

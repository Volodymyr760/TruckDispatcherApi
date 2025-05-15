using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TruckDispatcherApi.Models
{
    public class HeatmapState
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required string Id { get; set; }

        [Required]
        [StringLength(450)]
        public required string HeatmapId { get; set; }

        [Required]
        [StringLength(2)]
        public required string State { get; set; }

        /// <summary>
        /// Кількість Pickup вантажів для цього штату
        /// </summary>
        [Required]
        public int PickupsAmount { get; set; }

        /// <summary>
        /// Сума Rate усіх Pickup вантажів цього штату
        /// </summary>
        [Required]
        public decimal SumPickupRates { get; set; }

        /// <summary>
        /// Обчислюється за формулою Sum(Rate) / Sum(Miles) для усіх Pickup вантажів цього штату
        /// </summary>
        [Required]
        public decimal AveragePickupRate { get; set; }

        /// <summary>
        /// Кількість Delivery вантажів, які прибувають у цей штат
        /// </summary>
        [Required]
        public int DeliveriesAmount { get; set; }

        /// <summary>
        /// Сума Rate усіх Delivery вантажів, які прибувають у цей штат
        /// </summary>
        [Required]
        public decimal SumDeliveryRates { get; set; }

        /// <summary>
        /// Обчислюється за формулою Sum(Rate) / Sum(Miles) для усіх Delivery вантажів, які прибувають у цей штат
        /// </summary>
        [Required]
        public decimal AverageDeliveryRate { get; set; }

        /// <summary>
        /// Ранг цього HeatmapItem об’єкту в загальному об’єкті Heatmap.
        /// Температура ринку перевезень відображається у вигляді рангу, чим вища величина тим "гарячіший" ринок.
        /// Ранг може бути від 1 до 5 в залежності від діапазону значень величини SumPickupRates / SumDeliveryRates для кожного штату.
        /// Використовується на Frontend стороні для колоризації карти штатів - від "холодного" до "гарячого" кольорів.
        /// </summary>
        [Required]
        public int Ranq { get; set; }
    }
}

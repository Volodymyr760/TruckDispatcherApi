namespace TruckDispatcherApi.Services
{
    public class EquipmentProfitability
    {
        /// <summary>
        /// Profit / Rate * 100%
        /// </summary>
        public decimal All {  get; set; }

        public decimal Flatbed {  get; set; }

        public decimal Reefer { get; set; }

        public decimal Van { get; set; }
    }
}

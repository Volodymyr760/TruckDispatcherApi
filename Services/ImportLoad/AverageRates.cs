namespace TruckDispatcherApi.Services
{
    public class AverageRates
    {
        public DateTime DateTime { get; set; }

        public decimal All { get; set; }

        public decimal FlatbedRate { get; set; }

        public decimal ReeferRate { get; set; }

        public decimal VanRate { get; set; }
    }
}

namespace TruckDispatcherApi.Services
{
    public class WeekResults
    {
        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }

        public double TotalMiles { get; set; }

        public double MilesPerTruck { get; set; }

        public decimal TotalRate { get; set; }

        public decimal AverageRate { get; set; }

        public decimal TotalProfit { get; set; }

        public decimal TotalCosts { get; set; }
    }
}

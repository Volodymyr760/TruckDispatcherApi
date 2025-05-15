namespace TruckDispatcherApi.Services
{
    public class ImportLoadSearchParams<ImportloadDto> : SearchParams<ImportloadDto> where ImportloadDto : class
    {
        public required CityDto Origin { get; set; }

        public CityDto? Destination { get; set; }

        public required TruckDto Truck { get; set; }

        public DateTime PickupStartDate { get; set; }

        public double Deadhead {  get; set; }

        public double MilesMin { get; set; }

        public double MilesMax { get; set; }
    }
}

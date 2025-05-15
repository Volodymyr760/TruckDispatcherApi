namespace TruckDispatcherApi.Services
{
    public class LoadsByStatus
    {
        public int All { get; set; }

        /// <summary>
        /// Miles + DeadheadOrigin + DeadheadDestination
        /// </summary>
        public double AllLoadsMileage { get; set; }
        public decimal AllLoadsGross { get; set; }
        public decimal AllLoadsCosts { get; set; }

        public int SavedLoads { get; set; }
        public double SavedLoadsMileage { get; set; }
        public decimal SavedLoadsGross { get; set; }
        public decimal SavedLoadsCosts { get; set; }

        public int BookedLoads { get; set; }
        public double BookedLoadsMileage { get; set; }
        public decimal BookedLoadsGross { get; set; }
        public decimal BookedLoadsCosts { get; set; }

        public int InProgressLoads { get; set; }
        public double InProgressLoadsMileage { get; set; }
        public decimal InProgressLoadsGross { get; set; }
        public decimal InProgressLoadsCosts { get; set; }

        public int CompletedLoads { get; set; }
        public double CompletedLoadsMileage { get; set; }
        public decimal CompletedLoadsGross { get; set; }
        public decimal CompletedLoadsCosts { get; set; }

        public int PayedLoads { get; set; }
        public double PayedLoadsMileage { get; set; }
        public decimal PayedLoadsGross { get; set; }
        public decimal PayedLoadsCosts { get; set; }
    }
}

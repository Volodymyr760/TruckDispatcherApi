namespace TruckDispatcherApi.Services
{
    public class ParserFactory
    {
        const string LOAD_CENTRAL = "LoadCentral";
        const string TRUCKSMARTER = "TruckSmarter";
        const string TRULOS = "Trulos";

        public static IParser GetParser(string name)
        {
            switch (name)
            {
                case LOAD_CENTRAL:
                    return new ParserLoadCentral();
                default:
                    return new ParserTruckSmarter();
            }
        }
    }
}

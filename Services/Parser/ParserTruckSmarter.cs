using Newtonsoft.Json;
using System.Globalization;
using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Services
{
    public class ParserTruckSmarter() : IParser
    {
        public string Name { get; set; } = "TruckSmarter";

        public string RootLink { get; set; } = "https://app.trucksmarter.com/";

        public async Task<List<ImportLoadDto>> ParseAsync(string jsonString)
        {
            // reading json files based on https://stackoverflow.com/questions/19596135/reading-json-files-with-c-sharp-and-json-net
            dynamic? result = JsonConvert.DeserializeObject(jsonString);
            var loads = new List<ImportLoadDto>();

            if (result != null)
            {
                // todo: implement Parallel Task
                foreach (var load in result)
                {
                    var loadId = (string)load.id;
                    try
                    {
                        // Origin
                        double originLatitude = double.Parse((string)load.pickup.latitude, CultureInfo.InvariantCulture);
                        double originLongitude = double.Parse((string)load.pickup.longitude, CultureInfo.InvariantCulture);
                        var origin = (string)load.pickup.address.city + ", " + (string)load.pickup.address.state;

                        // Destination
                        double destinationLatitude = double.Parse((string)load.delivery.latitude, CultureInfo.InvariantCulture);
                        double destinationLongitude = double.Parse((string)load.delivery.longitude, CultureInfo.InvariantCulture);
                        var destination = (string)load.delivery.address.city + ", " + (string)load.delivery.address.state;

                        // Pickup / Delivery dates
                        if (!DateTime.TryParse((string)load.pickup.appointmentStartTime, new CultureInfo("en-US", false), out DateTime pickupDate)) continue;
                        if (!DateTime.TryParse((string)load.delivery.appointmentEndTime, new CultureInfo("en-US", false), out DateTime deliveryDate)) continue;

                        // Length, Weight
                        int.TryParse((string)load.equipment.length, new CultureInfo("en-US", false), out int length);
                        int.TryParse((string)load.weight, new CultureInfo("en-US", false), out int weight);

                        // Shipper info
                        string shipperName = (string)load.broker;
                        string? shipperEmail = (string)load.biddingEmail;
                        string? shipperPhone = (string)load.bookingPhoneNumber;
                        // Generally in the importLoad phone has format 'tel:+1-800-580-3101,,8,,7647391', correct it
                        if (!string.IsNullOrEmpty(shipperPhone))
                        {
                            if (shipperPhone.Contains(',')) shipperPhone = shipperPhone[..shipperPhone.IndexOf(",")];
                            shipperPhone = shipperPhone.Replace("tel:", "").Replace("-", "");
                        }

                        double miles = double.Parse((string)load.distance, CultureInfo.InvariantCulture);
                        if (!decimal.TryParse((string)load.price, CultureInfo.InvariantCulture, out decimal price)) continue;
                        if (!decimal.TryParse((string)load.ratePerMile, CultureInfo.InvariantCulture, out decimal ratePerMile)) continue;

                        // Equipment
                        var equipments = JsonConvert.DeserializeObject<string[]>(load.equipment.trailerTypes.ToString());

                        foreach (var item in equipments)
                        {
                            Equipment equipment = item switch
                            {
                                "Flatbed" => Equipment.Flatbed,
                                "Reefer" => Equipment.Reefer,
                                _ => Equipment.Van,
                            };

                            var newLoad = new ImportLoadDto()
                            {
                                ReferenceId = (string)load.brokerLoadId,
                                Origin = origin,
                                OriginLatitude = originLatitude,
                                OriginLongitude = originLongitude,
                                Destination = destination,
                                DestinationLatitude = destinationLatitude,
                                DestinationLongitude = destinationLongitude,
                                PickUp = pickupDate,
                                Delivery = deliveryDate,
                                Length = length,
                                Weight = weight,
                                Equipment = equipment,
                                ShipperName = shipperName,
                                ShipperEmail = shipperEmail,
                                ShipperPhone = shipperPhone,
                                ShipperLogo = null,
                                ShipperDotNumber = string.Empty,
                                ShipperMcNumber = string.Empty,
                                Miles = double.Round(miles, 0),
                                Rate = decimal.Round(price, 2),
                                RatePerMile = decimal.Round(ratePerMile, 2),
                                Requirements = string.Empty
                            };

                            loads.Add(newLoad);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Problem with load id:" + loadId + ", description: " + ex.Message);
                    }
                }
            }

            return loads;
        }
    }
}

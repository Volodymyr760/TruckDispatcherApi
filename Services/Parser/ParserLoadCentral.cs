using Newtonsoft.Json;
using System.Globalization;
using TruckDispatcherApi.Library;

namespace TruckDispatcherApi.Services
{
    public class ParserLoadCentral : IParser
    {
        public string Name { get; set; } = "Trulos";

        public string RootLink { get; set; } = "https://www.trulos.com/index.php";

        public async Task<List<ImportLoadDto>> ParseAsync(string jsonString)
        {
            // reading json files based on https://stackoverflow.com/questions/19596135/reading-json-files-with-c-sharp-and-json-net
            dynamic? result = JsonConvert.DeserializeObject(jsonString);
            var loads = new List<ImportLoadDto>();

            if (result != null)
            {
                foreach (var load in result)
                {
                    var loadId = (string)load.id;
                    try
                    {
                        var referenceId = (string)load.reference_id;

                        // Origin
                        var originGeoCoords = JsonConvert.DeserializeObject<string[]>(load.origin.location.geo.ToString());
                        double originLatitude = double.Parse((string)originGeoCoords[0], CultureInfo.InvariantCulture);
                        double originLongitude = double.Parse((string)originGeoCoords[1], CultureInfo.InvariantCulture);
                        var origin = (string)load.origin.location.address.city + ", " + (string)load.origin.location.address.state;

                        // Destination
                        var destinationGeoCoords = JsonConvert.DeserializeObject<string[]>(load.destination.location.geo.ToString());
                        double destinationLatitude = string.IsNullOrEmpty((string)destinationGeoCoords[0]) ? 0 : double.Parse((string)destinationGeoCoords[0], CultureInfo.InvariantCulture);
                        double destinationLongitude = string.IsNullOrEmpty((string)destinationGeoCoords[1]) ? 0 : double.Parse((string)destinationGeoCoords[1], CultureInfo.InvariantCulture);
                        if (string.IsNullOrEmpty((string)load.destination.location.address.state)) continue;
                        var destination = (string)load.destination.location.address.city + ", " + (string)load.destination.location.address.state;

                        // Pickup / Delivery dates
                        if (!DateTime.TryParse((string)load.origin.date, new CultureInfo("en-US", false), out DateTime pickupDate)) continue;
                        if (!DateTime.TryParse((string)load.destination.date, new CultureInfo("en-US", false), out DateTime deliveryDate)) continue;

                        int weight = 0;
                        try
                        {
                            if (!int.TryParse((string)load.measure.weight.value, new CultureInfo("en-US", false), out weight)) weight = 0;
                        }
                        catch { }

                        // Shipper info
                        string shipperName = (string)load.customer.company;
                        string? shipperEmail = (string)load.customer.email;
                        string? shipperPhone = (string)load.customer.phone;
                        // Generally LoadCentral phone has formats '615-610-1641 ext: 1', '800-444-9008' or '(224) 265-7450'
                        // correct it
                        if (!string.IsNullOrEmpty(shipperPhone))
                        {
                            if (shipperPhone.Contains(" ext")) shipperPhone = shipperPhone.Substring(0, shipperPhone.IndexOf(" ext"));
                            shipperPhone = "+1" + shipperPhone.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");
                        }

                        double miles = double.Parse((string)load.total_distance.value, CultureInfo.InvariantCulture);
                        if (!decimal.TryParse((string)load.rate.total, CultureInfo.InvariantCulture, out decimal price)) continue;
                        if (!decimal.TryParse((string)load.rate.per_mile, CultureInfo.InvariantCulture, out decimal ratePerMile)) continue;

                        // Equipment
                        var equipments = JsonConvert.DeserializeObject<string[]>(load.equipment_type.ToString());

                        foreach (var item in equipments) // flatbed, reefer or dry_van
                        {
                            Equipment equipment = item switch
                            {
                                "flatbed" => Equipment.Flatbed,
                                "reefer" => Equipment.Reefer,
                                _ => Equipment.Van,
                            };

                            var newLoad = new ImportLoadDto()
                            {
                                ReferenceId = referenceId,
                                Origin = origin,
                                OriginLatitude = originLatitude,
                                OriginLongitude = originLongitude,
                                Destination = destination,
                                DestinationLatitude = destinationLatitude,
                                DestinationLongitude = destinationLongitude,
                                PickUp = pickupDate,
                                Delivery = deliveryDate,
                                Length = 0,
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Controllers.ImportLoad
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin, Carrier, Broker")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class ImportLoadController(IBrokerService brokerService,
        ICityService cityService,
        IImportLoadService importLoadService) : AppControllerBase
    {
        private readonly IBrokerService brokerService = brokerService;
        private readonly ICityService cityService = cityService;
        private readonly IImportLoadService importLoadService = importLoadService;

        /// <summary>
        /// Changes Pickup and Delivery dates for test proposes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangePickupAndDeliveryDatesForTestsAsync(int days)
        {
            await importLoadService.ChangePickupAndDeliveryDatesForTestsAsync(days);
            return Ok();
        }

        /// <summary>
        /// Deletes an ImportLoad.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/importload/delete/33f84540-5e7c-474b-a030-77872d2738c6
        ///     
        /// </remarks>
        /// <param name="id">Identifier string id</param>
        /// <returns>Status 200</returns>
        /// <response code="200">Returns status 200</response>
        [HttpDelete]
        [Authorize(Roles = "Broker")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await importLoadService.DeleteAsync(id);
            return Ok();
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLegacyAsync()
        {
            await importLoadService.DeleteLegacy();
            return Ok();
        }

        /// <summary>
        /// Creates new ImportLoadDto.
        /// </summary>
        /// <returns>Status 201 and created ImportLoadDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/load/create
        ///     {
        ///        referenceId: "496063025",
        ///        origin": "New Hampton, IA",
        ///        originLatitude: 57.57,
        ///        originLongitude: 57.57,
        ///        destination: "Prosperity, SC",
        ///        destinationLatitude: 57.57,
        ///        destinationLongitude: 57.57,
        ///        pickUp: "2024-11-25T18:00:00.000Z",
        ///        delivery: "2024-11-27T17:45:00.000Z",
        ///        length: 53,
        ///        weight: 44000,
        ///        equipment: 2,
        ///        shipperId: "33f84540-5e7c-474b-a030-77872d2738c6",
        ///        shipperName: "navisphere",
        ///        shipperEmail: "westregionloadinquiries@venturelogistics.com",
        ///        shipperPhone: "tel:+1-888-491-6016",
        ///        shipperLogo: "",
        ///        shipperDotNumber: "111111",
        ///        shipperMcNumber: "222222",
        ///        miles: 1070.813324771944,
        ///        deadheadOrigin" 25,
        ///        deadheadDestination: 51,
        ///        rate: 2010,
        ///        ratePerMile: 1.8770778748276025,
        ///        profit: 1512.5,
        ///        profitPerMile: 1.45,
        ///        requirements: ""
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created ImportLoadDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="403">If the user hasn't need role</response>
        [HttpPost]
        [Authorize(Roles = "Broker")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAsync([FromBody] ImportLoadDto loadDto) =>
            Created("/api/importload/create", await importLoadService.CreateAsync(loadDto));

        /// <summary>
        /// Gets list of ImportLoadDto objects for broker-user
        /// </summary>  
        /// <param name="searchParams"></param>
        /// <returns>Status 200 and list of ImportLoadDto</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /api/importload/get
        ///     {
        ///        currentPage: 1,
        ///        pageSize: 100,
        ///        searchCriteria: "",
        ///        userId: "74d58e64-cfce-4d28-9df7-9aee4a7cc9b8",
        ///        sortField: "Origin",
        ///        order: 0,
        ///        itemList: [],
        ///        pageCount: 0,
        ///        totalItemsCount: 0,
        ///        equipment: 0
        ///     }
        ///     
        /// </remarks> 
        /// <response code="200">Returns BrokerLoadSearchParams with list of ImportLoadDto</response>
        /// <response code="400">If the argument is not valid or something went wrong</response>
        [HttpPost]
        [Authorize(Roles = "Broker")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAsync([FromBody] BrokerLoadSearchParams<ImportLoadDto> searchParams) =>
            Ok(await importLoadService.GetAsync(searchParams));

        [HttpGet]
        public async Task<IActionResult> GetAverageRatesAsync() => Ok(await importLoadService.GetAverageRatesAsync());

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<IActionResult> ImportLoadCentralLoadsAsync(IFormFile file)
        {
            try
            {
                StreamReader sr = new(file.OpenReadStream());
                var json = await sr.ReadToEndAsync();
                sr.Close();

                IParser parser = ParserFactory.GetParser("LoadCentral");
                List<ImportLoadDto> loads = await parser.ParseAsync(json);

                loads = await CheckAndFillShipperInfoAsync(loads);

                loads = await CheckCitiesExistingAsync(loads);

                foreach (var load in loads)
                {
                    try
                    {
                        // Check ImportLoad existing by ReferenceId + Equipment
                        var testImportLoad = await importLoadService.IsImportLoadExistsAsync(load);
                        if (testImportLoad != null) await importLoadService.DeleteAsync(testImportLoad.Id);

                        await importLoadService.CreateAsync(load);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Problem with load.ReferenceId: " + load.ReferenceId + ", exception: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseErrorFactory.GetInternalServerError(ex.Message));
            }

            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportTruckSmarterLoadsAsync(IFormFile file)
        {
            try
            {
                StreamReader sr = new(file.OpenReadStream());
                var json = await sr.ReadToEndAsync();
                sr.Close();

                IParser parser = ParserFactory.GetParser("TruckSmarter");
                List<ImportLoadDto> loads = await parser.ParseAsync(json);

                // Check ShipperInfo before creating Importloads
                loads = await CheckAndFillShipperInfoAsync(loads);

                loads = await CheckCitiesExistingAsync(loads);

                foreach (var load in loads)
                {
                    try
                    {
                        // Check ImportLoad existing by ReferenceId + Equipment
                        var testImportLoad = await importLoadService.IsImportLoadExistsAsync(load);
                        if (testImportLoad != null) await importLoadService.DeleteAsync(testImportLoad.Id);

                        await importLoadService.CreateAsync(load);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Problem with load.ReferenceId: " + load.ReferenceId + ", exception: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ResponseErrorFactory.GetInternalServerError(ex.Message));
            }

            return Ok();
        }

        /// <summary>
        /// Sends parameters to server in body to find list of available loads with counting Profit/ProfitPerMile fields
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>Status 200 and list of ImportLoadDto</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /api/importload/search
        ///     {
        ///        pageSize: 100,
        ///        currentPage: 1,
        ///        origin: "",
        ///        destination: "",
        ///        truck: "",
        ///        pickupStartDate: "",
        ///        deadheadOriginMax: "",
        ///        deadheadDestinationMax: "",
        ///        milesMin: 300,
        ///        milesMax: 3000,
        ///        sortField: "PickUp",
        ///        order: 0,
        ///        pageCount": 0,
        ///        totalItemCount": 0
        ///     }
        ///     
        /// </remarks> 
        /// <response code="200">Returns SearchImportLoadParams with list of ImportLoadDto</response>
        /// <response code="400">If the argument is not valid or something went wrong</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchAsync([FromBody] ImportLoadSearchParams<ImportLoadDto> searchParams)
        {
            var searchResult = await importLoadService.SearchAsync(searchParams);
            searchParams.ItemList = searchResult.ItemList.ToList();
            await CalculateProfitAsync(searchParams);

            // remove loads with deadheads bigger then value in searchParams
            searchParams.ItemList = searchParams.ItemList.Where(il => il.DeadheadOrigin < searchParams.Deadhead).ToList();
            if (searchParams.Destination != null)
                searchParams.ItemList = searchParams.ItemList.Where(il => il.DeadheadDestination < searchParams.Deadhead).ToList();

            searchParams.PageCount = Convert.ToInt32(Math.Ceiling((double)searchParams.ItemList.Count() / searchParams.PageSize));
            searchParams.TotalItemsCount = searchParams.ItemList.Count();

            searchParams.ItemList = searchParams.SortField switch
            {
                "Pickup" => searchParams.Order == OrderType.Ascending ? [.. searchParams.ItemList.OrderBy(il => il.PickUp)] :
                                        [.. searchParams.ItemList.OrderByDescending(il => il.PickUp)],
                "Delivery" => searchParams.Order == OrderType.Ascending ? [.. searchParams.ItemList.OrderBy(il => il.Delivery)] :
                                        [.. searchParams.ItemList.OrderByDescending(il => il.Delivery)],
                "Miles" => searchParams.Order == OrderType.Ascending ? [.. searchParams.ItemList.OrderBy(il => il.Miles)] :
                                        [.. searchParams.ItemList.OrderByDescending(il => il.Miles)],
                "Rate" => searchParams.Order == OrderType.Ascending ? [.. searchParams.ItemList.OrderBy(il => il.Rate)] :
                                        [.. searchParams.ItemList.OrderByDescending(il => il.Rate)],
                "Rate Per Mile" => searchParams.Order == OrderType.Ascending ? [.. searchParams.ItemList.OrderBy(il => il.RatePerMile)] :
                                        [.. searchParams.ItemList.OrderByDescending(il => il.RatePerMile)],
                "Profit Per Mile" => searchParams.Order == OrderType.Ascending ? [.. searchParams.ItemList.OrderBy(il => il.ProfitPerMile)] :
                                        [.. searchParams.ItemList.OrderByDescending(il => il.ProfitPerMile)],
                // Profit
                _ => searchParams.Order == OrderType.Ascending ? [.. searchParams.ItemList.OrderBy(il => il.Profit)] :
                                        searchParams.ItemList.OrderByDescending(il => il.Profit).ToList(),
            };
            searchParams.ItemList = searchParams.ItemList.Skip((searchParams.CurrentPage - 1) * searchParams.PageSize).Take(searchParams.PageSize).ToList();

            return Ok(searchParams);
        }

        /// <summary>
        /// Updates an existing ImportLoadDto item.
        /// </summary>
        /// <returns>Status 200 and updated ImportLoadDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/load/update
        ///     {
        ///        id: "aaa84540-bbbb-474b-a030-77872d2738c6",
        ///        referenceId: "496063025",
        ///        origin": "New Hampton, IA",
        ///        originLatitude: 57.57,
        ///        originLongitude: 57.57,
        ///        destination: "Prosperity, SC",
        ///        destinationLatitude: 57.57,
        ///        destinationLongitude: 57.57,
        ///        pickUp: "2024-11-25T18:00:00.000Z",
        ///        delivery: "2024-11-27T17:45:00.000Z",
        ///        length: 53,
        ///        weight: 44000,
        ///        equipment: 2,
        ///        shipperId: "33f84540-5e7c-474b-a030-77872d2738c6",
        ///        shipperName: "navisphere",
        ///        shipperEmail: "westregionloadinquiries@venturelogistics.com",
        ///        shipperPhone: "tel:+1-888-491-6016",
        ///        shipperLogo: "",
        ///        shipperDotNumber: "111111",
        ///        shipperMcNumber: "222222",
        ///        miles: 1070.813324771944,
        ///        deadheadOrigin" 25,
        ///        deadheadDestination: 51,
        ///        rate: 2010,
        ///        ratePerMile: 1.8770778748276025,
        ///        profit: 1512.5,
        ///        profitPerMile: 1.45,
        ///        requirements: ""
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated ImportLoadDto item</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPut]
        [Authorize(Roles = "Broker")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAsync([FromBody] ImportLoadDto loadDto) =>
            Ok(await importLoadService.UpdateAsync(loadDto));

        #region Helpers

        private async Task<CityDto?> AddNewCityFromLoadAsync(string fullCityName, double latitude, double longitude)
        {
            string[] nameAttributes = fullCityName.Split(", ");

            if (!cityService.IsStateAllowed(nameAttributes[1])) return null;

            string cityName = nameAttributes[0].Trim().ToLower();
            string[] cityNameParts = cityName.Split(" ");
            for (int i = 0; i < cityNameParts.Length; i++)
            {
                string word = cityNameParts[i];
                word = char.ToUpper(word[0]) + word[1..];// Capitalize the first letter
                cityNameParts[i] = word;
            }

            var newCity = new CityDto()
            {
                Name = string.Join(" ", cityNameParts),
                State = nameAttributes[1].ToUpper(),
                FullName = string.Join(" ", cityNameParts) + ", " + nameAttributes[1].ToUpper(),
                Latitude = latitude,
                Longitude = longitude
            };

            return await cityService.CreateAsync(newCity);
        }

        private async Task CalculateProfitAsync(ImportLoadSearchParams<ImportLoadDto> searchParams)
        {
            // todo: implement Parallel Task
            foreach (var load in searchParams.ItemList)
            {
                load.DeadheadOrigin = cityService.CalculateDistance(searchParams.Origin.Latitude, searchParams.Origin.Longitude,
                    load.OriginLatitude, load.OriginLongitude);
                if (searchParams.Destination != null)
                    load.DeadheadDestination = cityService.CalculateDistance(searchParams.Destination.Latitude, searchParams.Destination.Longitude,
                        load.DestinationLatitude, load.DestinationLongitude);

                var distance = (decimal)(load.DeadheadOrigin + load.Miles + load.DeadheadDestination);
                decimal totalCosts = searchParams.Truck.CostPerMile * distance;
                load.Profit = load.Rate - totalCosts;
                load.ProfitPerMile = load.Profit / distance;
                load.RatePerMile = load.Rate / distance;
            }
        }

        private async Task<List<ImportLoadDto>> CheckAndFillShipperInfoAsync(List<ImportLoadDto> loads)
        {
            foreach (var load in loads)
            {
                var parserBroker = await brokerService.GetByNameAsync(load.ShipperName);
                if (parserBroker != null)
                {
                    load.ShipperName = parserBroker.Name;
                    load.ShipperEmail ??= parserBroker.Email;
                    load.ShipperPhone ??= parserBroker.Phone;
                    load.ShipperLogo = parserBroker.Logo;
                    load.ShipperDotNumber = parserBroker.DotNumber;
                    load.ShipperMcNumber = parserBroker.McNumber;
                }
                else
                {
                    throw new Exception("No info found for broker: " + load.ShipperName);
                }
            }

            return loads;
        }

        private async Task<List<ImportLoadDto>> CheckCitiesExistingAsync(List<ImportLoadDto> loads)
        {
            List<ImportLoadDto> loadsWithExistingOriginAndDestination = [];
            foreach (var load in loads)
            {
                var originCityDto = await cityService.GetCityByFullNameAsync(load.Origin);
                var destinationCityDto = await cityService.GetCityByFullNameAsync(load.Destination);
                if (originCityDto != null && destinationCityDto != null)
                {
                    load.Origin = originCityDto.Name + ", " + originCityDto.State;
                    load.Destination = destinationCityDto.Name + ", " + destinationCityDto.State;
                    loadsWithExistingOriginAndDestination.Add(load);
                }
            }

            return loadsWithExistingOriginAndDestination;
        }

        #endregion
    }
}

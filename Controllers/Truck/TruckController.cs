using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.IO;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Controllers.Truck
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin, Carrier")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class TruckController(ITruckService truckService,
        IDriverService driverService,
        IImageService imageService,
        ILoadService loadService) : AppControllerBase
    {
        private readonly ITruckService truckService = truckService;
        private readonly IDriverService driverService = driverService;
        private readonly IImageService imageService = imageService;
        private readonly ILoadService loadService = loadService;

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetTrucksNumbersByStatusAsync([FromRoute] string userId) =>
            Ok(await truckService.GetTrucksNumbersByStatusAsync(userId));

        /// <summary>
        /// Searchs list of TruckDto
        /// </summary>
        /// <param name="truckSearchParams"></param>
        /// <returns>Status 200 and list of TruckDto</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/truck/search
        ///     {
        ///        currentPage: 1,
        ///        pageSize: 100,
        ///        searchCriteria: "",
        ///        userId: "74d58e64-cfce-4d28-9df7-9aee4a7cc9b8",
        ///        sortField: "Name",
        ///        order: 0,
        ///        includeNavProperties: 1,
        ///        itemList: [],
        ///        pageCount: 0,
        ///        totalItemsCount: 0,
        ///        equipment: 0,
        ///        truckStatus: 0
        ///     }
        ///     
        /// </remarks> 
        /// <response code="200">Returns list of TruckDto</response>
        /// <response code="400">If the argument is not valid or something went wrong</response>
        [HttpPost]
        public async Task<IActionResult> SearchAsync([FromBody] TruckSearchParams<TruckDto> truckSearchParams) =>
            Ok(await truckService.GetAsync(truckSearchParams));

        /// <summary>
        /// Gets a specified TruckDto item.
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Status OK and TruckDto item</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/truck/get/?id=516d8be0-3e18-4617-8781-86934b3333cd
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested TruckDto item</response>
        /// <response code="404">If the Truck with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] string id) =>
            Ok(await truckService.GetAsync(id));

        /// <summary>
        /// Creates a new Truck item.
        /// </summary>
        /// <returns>Status 201 and created TruckDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/truck/create
        ///     {
        ///        name: "Peterbilt 379 2007",
        ///        licensePlate: "12345TX",
        ///        equipment: 2,
        ///        costPerMile: 0.51,
        ///        avatar: "",
        ///        userId: "74d58e64-cfce-4d28-9df7-9aee4a7cc9b8",
        ///        truckStatus: 0,
        ///        notes: "Test note"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created TruckDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="403">If the user hasn't need role</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAsync([FromBody] TruckDto truckDto) =>
            Created("/api/truck/create", await truckService.CreateAsync(truckDto));

        /// <summary>
        /// Updates an existing Truck item.
        /// </summary>
        /// <returns>Status 200 and updated TruckDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/truck/update
        ///     {
        ///        id: "516d8be0-3e18-4617-8781-86934b3333cd",
        ///        name: "Peterbilt 379 2007",
        ///        licensePlate: "12345TX",
        ///        equipment: 2,
        ///        costPerMile: 0.51,
        ///        avatar: "",
        ///        userId: "74d58e64-cfce-4d28-9df7-9aee4a7cc9b8",
        ///        truckStatus: 0,
        ///        notes: "Test note"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated TruckDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the Option with given id not found</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] TruckDto truckDto) =>
            Ok(await truckService.UpdateAsync(truckDto));

        /// <summary>
        /// Deletes an Truck Item.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/truck/delete/33f84540-5e7c-474b-a030-77872d2738c6
        ///     
        /// </remarks>
        /// <param name="id">Identifier string id</param>
        /// <returns>Status 200</returns>
        /// <response code="200">Returns status 200</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var truck = await truckService.GetAsync(id);
            if (truck != null)
            {
                if (!string.IsNullOrEmpty(truck.Avatar)) await imageService.DeleteAsync(truck.Avatar);
                foreach (var load in truck.Loads) if (load.Id != null) await loadService.DeleteAsync(load.Id);
                foreach (var driver in truck.Drivers)
                    if (driver != null && driver.Id != null) await driverService.RemoveAssignedTruckAsync(driver.Id);
            }

            await truckService.DeleteAsync(id);

            return Ok();
        }
    }
}

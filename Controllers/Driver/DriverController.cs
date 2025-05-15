using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Controllers.Driver
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin, Carrier")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class DriverController(IDriverService driverService, IImageService imageService) : AppControllerBase
    {
        private readonly IDriverService driverService = driverService;
        private readonly IImageService imageService = imageService;

        /// <summary>
        /// Searchs list of DriverDto
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>Status 200 and list of DriverDto</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /api/driver/search
        ///     {
        ///        currentPage: 1,
        ///        pageSize: 100,
        ///        searchCriteria: "Name",
        ///        userId: "74d58e64-cfce-4d28-9df7-9aee4a7cc9b8",
        ///        sortField: "Name",
        ///        order: 0,
        ///        itemList: [],
        ///        pageCount: 0,
        ///        totalItemsCount: 0
        ///     }
        /// </remarks> 
        /// <response code="200">Returns list of DriverDto</response>
        /// <response code="400">If the argument is not valid or something went wrong</response>
        [HttpPost]
        public async Task<IActionResult> SearchAsync([FromBody] SearchParams<DriverDto> searchParams) =>
            Ok(await driverService.GetAsync(searchParams));

        /// <summary>
        /// Gets a specified DriverDto item.
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Status OK and DriverDto item</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/driver/get/?id=516d8be0-3e18-4617-8781-86934b3333cd
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested DriverDto item including assigned truck</response>
        /// <response code="404">If the Driver with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] string id) =>
            Ok(await driverService.GetAsync(id));

        /// <summary>
        /// Creates a new Driver. If driverDto contains truckId != null - returns driverDto including assigned truck
        /// </summary>
        /// <returns>Status 201 and created DriverDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/driver/create
        ///     {
        ///        userId: "74d58e64-cfce-4d28-9df7-9aee4a7cc9b8",
        ///        firstName: "Jaime",
        ///        lastName: "Lannister",
        ///        phone: "+1234567890",
        ///        email: "jaime@gmail.com",
        ///        avatar: null,
        ///        truckId: "597a0531-b91d-4a7d-a379-449fa3aa1a6a",
        ///        notes: null
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created DriverDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="403">If the user hasn't need role</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAsync([FromBody] DriverDto driverDto)
        {
            var createdDriver = await driverService.CreateAsync(driverDto);
            if (createdDriver.TruckId != null) createdDriver = await driverService.GetAsync(createdDriver.Id!);
            return Created("/api/driver/create", createdDriver);
        }

        /// <summary>
        /// Updates an existing Driver item.
        /// If driverDto contains truckId/truck - returns updated driverDto including this nested truck.
        /// </summary>
        /// <returns>Status 200 and updated DriverDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/driver/update
        ///     {
        ///        id: "516d8be0-3e18-4617-8781-86934b3333cd",
        ///        userId: "74d58e64-cfce-4d28-9df7-9aee4a7cc9b8",
        ///        firstName: "Jaime",
        ///        lastName: "Lannister",
        ///        phone: "+1234567890",
        ///        email: "jaime@gmail.com",
        ///        avatar: null,
        ///        notes: null
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated DriverDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the Driver with given id not found</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] DriverDto driverDto) =>
            Ok(await driverService.UpdateAsync(driverDto));

        /// <summary>
        /// Deletes a Driver.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/driver/delete/33f84540-5e7c-474b-a030-77872d2738c6
        ///     
        /// </remarks>
        /// <param name="id">Identifier string id</param>
        /// <returns>Status 200</returns>
        /// <response code="200">Returns status 200</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var driverToDelete = await driverService.GetAsync(id);
            if (driverToDelete != null)
            {
                if (!string.IsNullOrEmpty(driverToDelete.Avatar)) await imageService.DeleteAsync(driverToDelete.Avatar);
                await driverService.DeleteAsync(id);
            }

            return Ok();
        }
    }
}

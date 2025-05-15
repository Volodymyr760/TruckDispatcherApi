using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Controllers.City
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin, Carrier, Broker")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class CityController(ICityService cityService) : AppControllerBase
    {
        /// <summary>
        /// Search's list of CityDto
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>Status 200 and list of CityDto</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /api/city/search
        ///     {
        ///        currentPage: 1,
        ///        pageSize: 100,
        ///        searchCriteria: "",
        ///        sortField: "Name",
        ///        order: 0, // Sort direction: 0 - Ascending or 1 - Descending, 2 - None
        ///        itemList: [],
        ///        pageCount: 0,
        ///        totalItemsCount: 0
        ///     }
        /// </remarks> 
        /// <response code="200">Returns list of CityDto</response>
        /// <response code="400">If the argument is not valid or something went wrong</response>
        [HttpPost]
        public async Task<IActionResult> SearchAsync([FromBody] SearchParams<CityDto> searchParams) =>
            Ok(await cityService.GetAsync(searchParams));

        /// <summary>
        /// Gets a specified CityDto item.
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Status OK and CityDto item</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/city/get/?id=516d8be0-3e18-4617-8781-86934b3333cd
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested CityDto item</response>
        /// <response code="404">If the City with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] string id) =>
            Ok(await cityService.GetAsync(id));

        /// <summary>
        /// Gets CityDto item specified by full name.
        /// </summary>
        /// <param name="name">Full name (including state) of the city</param>
        /// <returns>Status OK and CityDto item</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/city/get/?name='Chicago, IL'
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested CityDto item</response>
        /// <response code="404">If the City with given full name not found</response>
        [HttpGet("{name}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNameAsync([FromRoute] string name) =>
            Ok(await cityService.GetCityByFullNameAsync(name));


        /// <summary>
        /// Get's list of CityDto duplicated by Name
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDuplicatesAsync() => Ok(await cityService.GetDuplicatesAsync());

        /// <summary>
        /// Creates a new City item.
        /// </summary>
        /// <returns>Status 201 and created CityDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/city/create
        ///     {
        ///        name: "Chicago",
        ///        state: "IL",
        ///        fullName: "Chicago, IL",
        ///        latitude: "41.885847",
        ///        longitude: "-87.618123"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created CityDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="403">If the user hasn't need role</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAsync([FromBody] CityDto cityDto) => 
            Created("/api/city/create", await cityService.CreateAsync(cityDto));

        /// <summary>
        /// Updates an existing City item.
        /// </summary>
        /// <returns>Status 200 and updated CityDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/city/update
        ///     {
        ///        id: "516d8be0-3e18-4617-8781-86934b3333cd",
        ///        name: "Chicago",
        ///        state: "IL",
        ///        fullName: "Chicago, IL",
        ///        latitude: "41.885847",
        ///        longitude: "-87.618123"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated CityDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the City with given id not found</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] CityDto cityDto) =>
            Ok(await cityService.UpdateAsync(cityDto));

        /// <summary>
        /// Deletes an City Item.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/city/delete/33f84540-5e7c-474b-a030-77872d2738c6
        ///     
        /// </remarks>
        /// <param name="id">Identifier string id</param>
        /// <returns>Status 200</returns>
        /// <response code="200">Returns status 200</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await cityService.DeleteAsync(id);
            return Ok();
        }
    }
}

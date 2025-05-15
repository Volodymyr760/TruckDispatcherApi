using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Controllers.Pricepackage
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin, Carrier, Broker")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class PricepackageController(IPricepackageService pricepackageService) : AppControllerBase
    {
        private readonly IPricepackageService pricepackageService = pricepackageService;

        /// <summary>
        /// Gets a list of PricepackageDto's with pagination params and field name for sorting.
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>Status 200 and list of ContactDto's</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /api/pricepackage/search
        ///     {
        ///        currentPage: 1,
        ///        pageSize: 100,
        ///        searchCriteria: "",
        ///        userId: "",
        ///        sortField: "Name",
        ///        order: 0,
        ///        itemList: [],
        ///        pageCount: 0,
        ///        totalItemsCount: 0
        ///     }
        /// </remarks>
        /// <response code="200">List of PricepackageDto's</response>
        [HttpPost]
        public async Task<IActionResult> SearchAsync([FromBody] SearchParams<PricepackageDto> searchParams) =>
            Ok(await pricepackageService.GetAsync(searchParams));

        /// <summary>
        /// Gets a specific PricepackageDto Item.
        /// </summary>
        /// <param name="id">Identifier string id</param>
        /// <returns>OK and PricepackageDto</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/pricepackage/get/?id=516d8be0-3e18-4617-8781-86934b3333cd
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested PricepackageDto item</response>
        /// <response code="404">If the Pricepackage with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] string id) =>
            Ok(await pricepackageService.GetAsync(id));

        /// <summary>
        /// Creates a new Pricepackage item.
        /// </summary>
        /// <returns>Status 201 and created PricepackageDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/pricepackage/create
        ///     {
        ///        name: "Free",
        ///        price: 1,
        ///        period: 3,
        ///        description: "Test description of Free price package",
        ///        posibilities: "10 GB of shared storage per user|Security and management features|Extended support (paid upgrade to premium support available)"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created PricepackageDto item</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] PricepackageDto pricepackageDto) =>
            Created("/api/pricepackage/create", await pricepackageService.CreateAsync(pricepackageDto));

        /// <summary>
        /// Updates an existing Pricepackage item.
        /// </summary>
        /// <returns>Status 200 and updated PricepackageDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/pricepackage/update
        ///     {
        ///        name: "Free",
        ///        price: "0.10",
        ///        period: 3,
        ///        description: "Test description of Free price package",
        ///        posibilities: "10 GB of shared storage per user|Security and management features|Extended support (paid upgrade to premium support available)"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated PricepackageDto item</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAsync([FromBody] PricepackageDto pricepackageDto) =>
            Ok(await pricepackageService.UpdateAsync(pricepackageDto));

        /// <summary>
        /// Deletes an Pricepackage item.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/pricepackage/delete?id=33f84540-5e7c-474b-a030-77872d2738c6
        ///     
        /// </remarks>
        /// <param name="id">Identifier string id</param>
        /// <returns>Status 200</returns>
        /// <response code="200">Returns status 200</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync([FromQuery] string id)
        {
            await pricepackageService.DeleteAsync(id);

            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Controllers.Load
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin, Carrier, Broker")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class LoadController(ILoadService loadService) : AppControllerBase
    {
        private readonly ILoadService loadService = loadService;

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetLoadsNumbersByStatusAsync([FromRoute] string userId) =>
            Ok(await loadService.GetLoadsNumbersByStatusAsync(userId));

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWeekResultsAsync([FromRoute] string userId) =>
            Ok(await loadService.GetWeekResultsAsync(userId));

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetEquipmentProfitabilityAsync([FromRoute] string userId) =>
            Ok(await loadService.GetEquipmentProfitabilityAsync(userId));

        /// <summary>
        /// Searchs list of LoadDto
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>Status 200 and list of LoadDto</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /api/load/search
        ///     {
        ///        currentPage: 1,
        ///        pageSize: 100,
        ///        searchCriteria: ["Origin or Destination"],
        ///        userId: "74d58e64-cfce-4d28-9df7-9aee4a7cc9b8",
        ///        sortField: "PickUp",
        ///        order: 0, // Sort direction: 0 - Ascending or 1 - Descending, 2 - None
        ///        itemList: [],
        ///        pageCount: 0,
        ///        totalItemsCount: 0
        ///        equipment: 0,
        ///        loadStatus: 0
        ///     }
        /// </remarks> 
        /// <response code="200">Returns list of LoadDto</response>
        /// <response code="400">If the argument is not valid or something went wrong</response>
        [HttpPost]
        public async Task<IActionResult> SearchAsync([FromBody] LoadSearchParams<LoadDto> searchParams) =>
            Ok(await loadService.GetAsync(searchParams));

        /// <summary>
        /// Gets a specified LoadDto item.
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Status OK and LoadDto item</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/load/get/?id=516d8be0-3e18-4617-8781-86934b3333cd
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested LoadDto item</response>
        /// <response code="404">If the Driver with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] string id) =>
            Ok(await loadService.GetAsync(id));

        /// <summary>
        /// Creates a new Load.
        /// </summary>
        /// <returns>Status 201 and created LoadDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/load/create
        ///     {
        ///        referenceId: "496063025",
        ///        origin": "New Hampton, IA",
        ///        destination: "Prosperity, SC",
        ///        pickUp: "2024-11-25T18:00:00.000Z",
        ///        delivery: "2024-11-27T17:45:00.000Z",
        ///        equipment: 2,
        ///        shipperName: "navisphere",
        ///        shipperEmail: "westregionloadinquiries@venturelogistics.com",
        ///        shipperPhone: "tel:+1-888-491-6016",
        ///        miles: 1070.813324771944,
        ///        deadheadOrigin" 25.5,
        ///        deadheadDestination: 51.4,
        ///        rate: 2010,
        ///        ratePerMile: 1.8770778748276025,
        ///        profit: 1512.5,
        ///        profitPerMile: 1.45,
        ///        requirements: ""
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created LoadDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="403">If the user hasn't need role</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAsync([FromBody] LoadDto loadDto) =>
            Created("/api/load/create", await loadService.CreateAsync(loadDto));

        /// <summary>
        /// Updates an existing Load item.
        /// </summary>
        /// <returns>Status 200 and updated LoadDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/load/update
        ///     {
        ///        id: "aaa84540-bbbb-474b-a030-77872d2738c6",
        ///        referenceId: "496063025",
        ///        origin": "New Hampton, IA",
        ///        destination: "Prosperity, SC",
        ///        pickUp: "2024-11-25T18:00:00.000Z",
        ///        delivery: "2024-11-27T17:45:00.000Z",
        ///        equipment: 2,
        ///        shipperName: "navisphere",
        ///        shipperEmail: "westregionloadinquiries@venturelogistics.com",
        ///        shipperPhone: "tel:+1-888-491-6016",
        ///        miles: 1070.813324771944,
        ///        deadheadOrigin" 25.5,
        ///        deadheadDestination: 51.4,
        ///        rate: 2010,
        ///        ratePerMile: 1.8770778748276025,
        ///        profit: 1512.5,
        ///        profitPerMile: 1.45,
        ///        requirements: ""
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated LoadDto item</response>
        /// <response code="400">If the argument is not valid</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAsync([FromBody] LoadDto loadDto) =>
            Ok(await loadService.UpdateAsync(loadDto));

        /// <summary>
        /// Deletes an Load.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/load/delete/33f84540-5e7c-474b-a030-77872d2738c6
        ///     
        /// </remarks>
        /// <param name="id">Identifier string id</param>
        /// <returns>Status 200</returns>
        /// <response code="200">Returns status 200</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id)
        {
            await loadService.DeleteAsync(id);

            return Ok();
        }

        /// <summary>
        /// Partly updates an existing Load item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <param name="patchDocument">Json Patch Document as array of operations</param>
        /// <returns>Status 200 and updated LoadDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH /api/load/partialupdate/{id}
        ///     [
        ///         {
        ///             "op": "replace",
        ///             "path": "/LoadStatus",
        ///             "value": 4
        ///         }
        ///     ]
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated LoadDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the form with given id not found</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PartialUpdateAsync(string id, JsonPatchDocument<object> patchDocument)
        {
            var updatedLoad = await loadService.PartialUpdateAsync(id, patchDocument);

            return Ok(updatedLoad);
        }

    }
}

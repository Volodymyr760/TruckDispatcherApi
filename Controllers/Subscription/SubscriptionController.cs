using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Controllers.Subscription
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin, Carrier, Broker")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class SubscriptionController(ISubscriptionService subscriptionService) : AppControllerBase
    {
        private readonly ISubscriptionService subscriptionService = subscriptionService;

        /// <summary>
        /// Search's list of SubscriptionDto
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>Status 200 and list of SubscriptionDto</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /api/subscription/search
        ///     {
        ///        "currentPage": 1,
        ///        "pageSize": 100,
        ///        "searchCriteria": "",
        ///        "sortField": "Name",
        ///        "order": 0, // Sort direction: 0 - Ascending or 1 - Descending, 2 - None
        ///        "itemList": []
        ///     }
        /// </remarks> 
        /// <response code="200">Returns list of SubscriptionDto</response>
        /// <response code="400">If the argument is not valid or something went wrong</response>
        [HttpPost]
        public async Task<IActionResult> SearchAsync([FromBody] SearchParams<SubscriptionDto> searchParams) =>
            Ok(await subscriptionService.GetAsync(searchParams));

        /// <summary>
        /// Gets a specified SubscriptionDto item.
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Status OK and SubscriptionDto item</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/subscription/get/?id=516d8be0-3e18-4617-8781-86934b3333cd
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested SubscriptionDto item</response>
        /// <response code="404">If the Subscription with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] string id) =>
            Ok(await subscriptionService.GetAsync(id));

        /// <summary>
        /// Creates a new Subscription item.
        /// </summary>
        /// <returns>Status 201 and created SubscriptionDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/subscription/create
        ///     {
        ///        "name": "Chicago, IL",
        ///        "latitude": "41.885847",
        ///        "longitude": "-87.618123"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created SubscriptionDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="403">If the user hasn't need role</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAsync([FromBody] SubscriptionDto subscriptionDto) =>
            Created("/api/subscription/create", await subscriptionService.CreateAsync(subscriptionDto));

        /// <summary>
        /// Updates an existing Subscription item.
        /// </summary>
        /// <returns>Status 200 and updated SubscriptionDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/subscription/update
        ///     {
        ///        "id": "516d8be0-3e18-4617-8781-86934b3333cd",
        ///        "name": "Chicago, IL",
        ///        "latitude": "41.885847",
        ///        "longitude": "-87.618123"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated SubscriptionDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the Subscription with given id not found</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] SubscriptionDto subscriptionDto) =>
            Ok(await subscriptionService.UpdateAsync(subscriptionDto));

        /// <summary>
        /// Deletes an Subscription Item.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/subscription/delete/33f84540-5e7c-474b-a030-77872d2738c6
        ///     
        /// </remarks>
        /// <param name="id">Identifier string id</param>
        /// <returns>Status 200</returns>
        /// <response code="200">Returns status 200</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await subscriptionService.DeleteAsync(id);
            return Ok();
        }
    }
}

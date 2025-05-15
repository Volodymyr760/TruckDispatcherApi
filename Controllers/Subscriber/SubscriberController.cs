using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Controllers.Subscriber
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin, Carrier, Broker")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class SubscriberController(ISubscriberService subscriberService) : AppControllerBase
    {
        private readonly ISubscriberService subscriberService = subscriberService;

        /// <summary>
        /// Search's list of SubscriberDto
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>Status 200 and list of SubscriberDto</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /api/subscriber/search
        ///     {
        ///        "currentPage": 1,
        ///        "pageSize": 100,
        ///        "searchCriteria": "Part of Email",
        ///        "sortField": "Email or CreatedAt",
        ///        "order": 0, // Sort direction: 0 - Ascending or 1 - Descending, 2 - None
        ///        "itemList": []
        ///     }
        /// </remarks> 
        /// <response code="200">Returns list of SubscriberDto</response>
        /// <response code="400">If the argument is not valid or something went wrong</response>
        [HttpPost]
        public async Task<IActionResult> SearchAsync([FromBody] SearchParams<SubscriberDto> searchParams) =>
            Ok(await subscriberService.GetAsync(searchParams));

        /// <summary>
        /// Gets a specified SubscriberDto item.
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Status OK and SubscriberDto item</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/subscriber/get/?id=516d8be0-3e18-4617-8781-86934b3333cd
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested SubscriberDto item</response>
        /// <response code="404">If the Subscriber with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] string id) =>
            Ok(await subscriberService.GetAsync(id));

        /// <summary>
        /// Creates a new Subscriber item.
        /// </summary>
        /// <returns>Status 201 and created SubscriberDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/subscriber/create
        ///     {
        ///        "email": "john@gmail.com",
        ///        "createdAt": "12/12/2024"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created SubscriberDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="403">If the user hasn't need role</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAsync([FromBody] SubscriberDto subscriberDto) =>
            Created("/api/subscriber/create", await subscriberService.CreateAsync(subscriberDto));

        /// <summary>
        /// Updates an existing Subscriber item.
        /// </summary>
        /// <returns>Status 200 and updated SubscriberDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/subscriber/update
        ///     {
        ///        "id": "516d8be0-3e18-4617-8781-86934b3333cd",
        ///        "email": "john@gmail.com",
        ///        "createdAt": "12/12/2024"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated SubscriberDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the Subscriber with given id not found</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] SubscriberDto subscriberDto) =>
            Ok(await subscriberService.UpdateAsync(subscriberDto));

        /// <summary>
        /// Deletes an Subscriber Item.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/subscriber/delete/33f84540-5e7c-474b-a030-77872d2738c6
        ///     
        /// </remarks>
        /// <param name="id">Identifier string id</param>
        /// <returns>Status 200</returns>
        /// <response code="200">Returns status 200</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await subscriberService.DeleteAsync(id);
            return Ok();
        }
    }
}

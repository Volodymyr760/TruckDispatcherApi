using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Controllers.Notification
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin, Carrier, Broker")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class NotificationController(INotificationService notificationService) : AppControllerBase
    {
        private readonly INotificationService notificationService = notificationService;

        /// <summary>
        /// Search's list of NotificationDto
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>Status 200 and list of NotificationDto</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /api/notification/search
        ///     {
        ///        "currentPage": 1,
        ///        "pageSize": 100,
        ///        "searchCriteria": "",
        ///        "userId" = "74d58e64-cfce-4d28-9df7-9aee4a7cc9b8"
        ///        "sortField": "CreatedAt",
        ///        "order": 0, // Sort direction: 0 - Ascending or 1 - Descending, 2 - None
        ///        "itemList": []
        ///     }
        /// </remarks> 
        /// <response code="200">Returns list of CityDto</response>
        /// <response code="400">If the argument is not valid or something went wrong</response>
        [HttpPost]
        public async Task<IActionResult> SearchAsync([FromBody] SearchParams<NotificationDto> searchParams) =>
            Ok(await notificationService.GetAsync(searchParams));

        /// <summary>
        /// Gets a specified NotificationDto item.
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Status OK and NotificationDto item</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/notification/get/?id=516d8be0-3e18-4617-8781-86934b3333cd
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested NotificationDto item</response>
        /// <response code="404">If the City with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] string id) =>
            Ok(await notificationService.GetAsync(id));

        /// <summary>
        /// Creates a new Notification item.
        /// </summary>
        /// <returns>Status 201 and created NotificationDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/notification/create
        ///     {
        ///        "senderAvatarUrl": "https://truckdispatcher.com/Uploads/Images/5d128cd7a124462086c8f0ab9e8b0b32.jpg",
        ///        "senderFullName": "John Snow",
        ///        "message": "Test message",
        ///        "isRead": "false",
        ///        "callBackUrl": "https://truckdispatcher.com/1",
        ///        "createdAt": "2022/12/31"
        ///        "recipientId": "e00875e2-e9f8-4e32-9db6-613e11aa7ff6",
        ///        "recipientEmail": "logisticmaster.2000@gmail.com"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created NotificationDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="403">If the user hasn't need role</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAsync(NotificationDto notificationDto) =>
            Created("/api/city/create", await notificationService.CreateAsync(notificationDto));

        /// <summary>
        /// Partly updates an existing Notification item.
        /// </summary>
        /// <param name="id">Identifier int id</param>
        /// <param name="patchDocument">Json Patch Document as array of operations</param>
        /// <returns>Status 200 and updated NotificationDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH /api/notification/partialupdate/{id}
        ///     [
        ///         {
        ///             "op": "replace",
        ///             "path": "/isRread",
        ///             "value": "true"
        ///         }
        ///     ]
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated Notification item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the Notification with given id not found</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PartialUpdateAsync(string id, JsonPatchDocument<object> patchDocument)
        {
            try { return Ok(await notificationService.PartialUpdateAsync(id, patchDocument)); }
            catch { return BadRequest(responseBadRequestError); }
        }

        /// <summary>
        /// Deletes an Notification Item.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/notification/delete/33f84540-5e7c-474b-a030-77872d2738c6
        ///     
        /// </remarks>
        /// <param name="id">Identifier string id</param>
        /// <returns>Status 200</returns>
        /// <response code="200">Returns status 200</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await notificationService.DeleteAsync(id);
            return Ok();
        }
    }
}

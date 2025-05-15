using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Controllers.MailTemplate
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class MailTemplateController(IMailTemplateService mailTemplateService) : AppControllerBase
    {
        private readonly IMailTemplateService mailTemplateService = mailTemplateService;

        /// <summary>
        /// Search's list of MailTemplateDto
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>Status 200 and list of MailTemplateDto</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /api/mailTemplate/search
        ///     {
        ///        "currentPage": 1,
        ///        "pageSize": 100,
        ///        "searchCriteria": "",
        ///        "sortField": "Id",
        ///        "order": 0, // Sort direction: 0 - Ascending or 1 - Descending, 2 - None
        ///        "itemList": []
        ///     }
        /// </remarks> 
        /// <response code="200">Returns list of MailTemplateDto</response>
        /// <response code="400">If the argument is not valid or something went wrong</response>
        [HttpPost]
        public async Task<IActionResult> SearchAsync([FromBody] SearchParams<MailTemplateDto> searchParams) =>
            Ok(await mailTemplateService.GetAsync(searchParams));

        /// <summary>
        /// Creates a new MailTemplate item.
        /// </summary>
        /// <returns>Status 201 and created MailTemplateDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/mailtemplate/create
        ///     {
        ///        "mailTemplateKey": 7,
        ///        "messageHtml": "Test message html",
        ///        "messagePlainText": "Added a new comment",
        ///        "subject": "Added a new comment."
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created MailTemplateDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="403">If the user hasn't need role</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAsync(MailTemplateDto mailTemplateDto ) => 
            Created("/api/city/create", await mailTemplateService.CreateAsync(mailTemplateDto));

        /// <summary>
        /// Updates an existing MailTemplate item.
        /// </summary>
        /// <returns>Status 200 and updated MailTemplateDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/mailTemplate/update
        ///     {
        ///        "id": "516d8be0-3e18-4617-8781-86934b3333cd",
        ///        "mailTemplateKey": 0,
        ///        "messageHtml": "Test message html",
        ///        "messagePlainText": "Test message plain text",
        ///        "subject": "Test subject"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated MailTemplateDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the MailTemplate with given id not found</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] MailTemplateDto mailTemplateDto) =>
            Ok(await mailTemplateService.UpdateAsync(mailTemplateDto));

        /// <summary>
        /// Deletes an MailTemplate Item.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/mailTemplate/delete/33f84540-5e7c-474b-a030-77872d2738c6
        ///     
        /// </remarks>
        /// <param name="id">Identifier string id</param>
        /// <returns>Status 200</returns>
        /// <response code="200">Returns status 200</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await mailTemplateService.DeleteAsync(id);
            return Ok();
        }
    }
}

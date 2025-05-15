using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Controllers.Client
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    //[Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class ClientController(IClientService clientService) : AppControllerBase
    {
        private readonly IClientService clientService = clientService;

        /// <summary>
        /// Searchs list of ClientDto
        /// </summary>
        /// <param name="clientSearchParams"></param>
        /// <returns>Status 200 and list of ClientDto</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/client/search
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
        ///        clientStatus: 0,
        ///        appRoles: 0
        ///     }
        ///     
        /// </remarks> 
        /// <response code="200">Returns list of ClientDto</response>
        /// <response code="400">If the argument is not valid or something went wrong</response>
        [HttpPost]
        public async Task<IActionResult> SearchAsync([FromBody] ClientSearchParams<ClientDto> clientSearchParams) =>
            Ok(await clientService.GetAsync(clientSearchParams));

        /// <summary>
        /// Gets a specified ClientDto item.
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Status OK and ClientDto item</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/client/get/?id=516d8be0-3e18-4617-8781-86934b3333cd
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested ClientDto item</response>
        /// <response code="404">If the Client with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] string id) =>
            Ok(await clientService.GetAsync(id));

        /// <summary>
        /// Creates a new Client item.
        /// </summary>
        /// <returns>Status 201 and created ClientDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/client/create
        ///     {
        ///        name: "Test Carrier",
        ///        email: "carrier@gmail.com",
        ///        city: "Chicago, IL",
        ///        clientStatus: 0,
        ///        appRoles: 2,
        ///        dotNumber: "123456",
        ///        createdAt: "2025-03-27 09:34:51",
        ///        invitedAt: null,
        ///        notes: "Test note"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created ClientDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="403">If the user hasn't need role</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAsync([FromBody] ClientDto clientDto) =>
            Created("/api/client/create", await clientService.CreateAsync(clientDto));

        /// <summary>
        /// Updates an existing Client item.
        /// </summary>
        /// <returns>Status 200 and updated ClientDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/client/update
        ///     {
        ///        id: "516d8be0-3e18-4617-8781-86934b3333cd",
        ///        name: "Test Carrier",
        ///        email: "carrier@gmail.com",
        ///        city: "Chicago, IL",
        ///        clientStatus: 0,
        ///        appRoles: 2,
        ///        dotNumber: "123456",
        ///        createdAt: "2025-03-27 09:34:51",
        ///        invitedAt: null,
        ///        notes: "Test note"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated ClientDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the Option with given id not found</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] ClientDto clientDto) =>
            Ok(await clientService.UpdateAsync(clientDto));

        /// <summary>
        /// Deletes an Client Item.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/client/delete/33f84540-5e7c-474b-a030-77872d2738c6
        ///     
        /// </remarks>
        /// <param name="id">Identifier string id</param>
        /// <returns>Status 200</returns>
        /// <response code="200">Returns status 200</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await clientService.DeleteAsync(id);
            return Ok();
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TruckDispatcherApi.Services;
using TruckDispatcherApi.Services.Common;

namespace TruckDispatcherApi.Controllers.ParserBroker
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class BrokerController(IBrokerService brokerService, IImageService imageService) : AppControllerBase
    {
        private readonly IBrokerService brokerService = brokerService;
        private readonly IImageService imageService = imageService;

        /// <summary>
        /// Searchs list of ParserBrokerDto,
        /// Available sort fields: ParserName, Name or ShortName
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>Status 200 and list of ParserBrokerDto</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /api/parserBroker/search
        ///     {
        ///        currentPage: 1,
        ///        pageSize: 10,
        ///        searchCriteria: "Name",
        ///        userId: "",
        ///        sortField: "Name",
        ///        order: 0,
        ///        itemList: [],
        ///        pageCount: 0,
        ///        totalItemsCount: 0
        ///     }
        /// </remarks> 
        /// <response code="200">Returns list of ParserBrokerDto</response>
        /// <response code="400">If the argument is not valid or something went wrong</response>
        [HttpPost]
        public async Task<IActionResult> SearchAsync([FromBody] SearchParams<BrokerDto> searchParams) =>
            Ok(await brokerService.GetAsync(searchParams));

        /// <summary>
        /// Gets a specified ParserBrokerDto item.
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Status OK and ParserBrokerDto item</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/parserBroker/get/?id=516d8be0-3e18-4617-8781-86934b3333cd
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested ParserBrokerDto item</response>
        /// <response code="404">If the ParserBroker with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] string id) =>
            Ok(await brokerService.GetAsync(id));

        /// <summary>
        /// Gets a specified by ShortName (from importLoad) ParserBrokerDto item.
        /// </summary>
        /// <param name="brokerShortName">ParserBroker's ShortName</param>
        /// <returns>Status OK and ParserBrokerDto item</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/parserBroker/getByName/?brokerShortName=navisphere
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested ParserBrokerDto item</response>
        /// <response code="404">If the ParserBroker with given id not found</response>
        [HttpGet("{brokerShortName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNameAsync([FromRoute] string brokerShortName) =>
            Ok(await brokerService.GetByNameAsync(brokerShortName));

        /// <summary>
        /// Creates a new ParserBroker.
        /// </summary>
        /// <returns>Status 201 and created ParserBrokerDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/parserBroker/create
        ///     {
        ///        parserName: "TruckSmarter",
        ///        name: "CH Robinson Company LLC",
        ///        shortName: "navisphere",
        ///        email: "loaddocs@chrobinson.com",
        ///        phone: "+18003269977",
        ///        dotNumber: "2211804",
        ///        mcNumber: "131029",
        ///        notes: null
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created ParserBrokerDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="403">If the user hasn't need role</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAsync([FromBody] BrokerDto brokerDto) =>
            Created("/api/parserBroker/create", await brokerService.CreateAsync(brokerDto));

        /// <summary>
        /// Updates an existing ParserBroker item.
        /// </summary>
        /// <returns>Status 200 and updated ParserBrokerDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/parserBroker/update
        ///     {
        ///        id: "516d8be0-3e18-4617-8781-86934b3333cd",
        ///        parserName: "TruckSmarter",
        ///        name: "CH Robinson Company LLC",
        ///        shortName: "navisphere",
        ///        email: "loaddocs@chrobinson.com",
        ///        phone: "+18003269977",
        ///        dotNumber: "2211804",
        ///        mcNumber: "131029",
        ///        notes: null
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated ParserBrokerDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the ParserBroker with given id not found</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] BrokerDto brokerDto) =>
            Ok(await brokerService.UpdateAsync(brokerDto));

        /// <summary>
        /// Deletes a ParserBroker.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/parserBroker/delete/33f84540-5e7c-474b-a030-77872d2738c6
        ///     
        /// </remarks>
        /// <param name="id">Identifier string id</param>
        /// <returns>Status 200</returns>
        /// <response code="200">Returns status 200</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var brokerToDelete = await brokerService.GetAsync(id);
            if (brokerToDelete != null)
            {
                // remove logo
                if (!string.IsNullOrEmpty(brokerToDelete.Logo)) await imageService.DeleteAsync(brokerToDelete.Logo);
                // remove importLoads - no needed because importLoads are updated on daily basis

                await brokerService.DeleteAsync(id);
            }
            return Ok();
        }
    }
}

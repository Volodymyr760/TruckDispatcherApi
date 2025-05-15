using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Controllers.Invoice
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin, Carrier, Broker")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class InvoiceController(IInvoiceService invoiceService, IUserService userService, IConfiguration configuration) : AppControllerBase
    {
        private readonly IInvoiceService invoiceService = invoiceService;
        private readonly IUserService userService = userService;
        private readonly IConfiguration configuration = configuration;

        /// <summary>
        /// Searchs list of of InvoiceDto's.
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>Status 200 and list of InvoiceDto's</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /api/driver/search
        ///     {
        ///        currentPage: 1,
        ///        pageSize: 10,
        ///        searchCriteria: "",
        ///        userId: "74d58e64-cfce-4d28-9df7-9aee4a7cc9b8",
        ///        sortField: "Invoice No",
        ///        order: 1,
        ///        itemList: [],
        ///        pageCount: 0,
        ///        totalItemsCount: 0
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">List of InvoiceDto's</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchAsync([FromBody] SearchParams<InvoiceDto> searchParams) =>
            Ok(await invoiceService.GetAsync(searchParams));

        /// <summary>
        /// Gets all InvoiceDto's with pagination params and values for search and sorting, without filtering by userId (Admin area).
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>Status 200 and list of InvoiceDto's</returns>
        /// <remarks>
        /// Sample request:
        ///     POST /api/driver/search
        ///     {
        ///        currentPage: 1,
        ///        pageSize: 10,
        ///        searchCriteria: "",
        ///        userId: "",
        ///        sortField: "Invoice No",
        ///        order: 0,
        ///        itemList: []
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">List of InvoiceDto's</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync([FromBody] SearchParams<InvoiceDto> searchParams) =>
            Ok(await invoiceService.GetAsync(searchParams));

        /// <summary>
        /// Gets a specific InvoiceDto Item.
        /// </summary>
        /// <param name="id">Identifier string id</param>
        /// <returns>OK and InvoiceDto</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/invoice/get/?id=516d8be0-3e18-4617-8781-86934b3333cd
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested InvoiceDto item</response>
        /// <response code="404">If the Invoice with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] string id) =>
            Ok(await invoiceService.GetAsync(id));

        /// <summary>
        /// Creates a new Invoice item.
        /// </summary>
        /// <returns>Status 201 and created InvoiceDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/invoice/create
        ///     {
        ///        invoiceTo: "Company 1",
        ///        item: "Price package Optimal",
        ///        quantity: "3",
        ///        price: "11.95",
        ///        userId: "e00875e2-e9f8-4e32-9db6-613e11aa7ff6"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created InvoiceDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="403">If the user hasn't need role</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateInvoiceDto createInvoiceDto)
        {
            var user = await userService.GetByEmailAsync(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (user == null) return NotFound(responseNotFoundError);

            var invoiceToCreate = new InvoiceDto
            {
                InvoiceNo = await invoiceService.GenerateInvoiceNumber(),
                InvoiceTo = createInvoiceDto.InvoiceTo,
                Item = createInvoiceDto.Item,
                Quantity = createInvoiceDto.Quantity,
                Price = createInvoiceDto.Price,
                Total = createInvoiceDto.Price * createInvoiceDto.Quantity,
                Beneficiary = configuration["Beneficiary:Name"],
                Account = configuration["Beneficiary:Account"],
                BeneficiaryEmail = configuration["Beneficiary:Email"],
                Bank = configuration["Beneficiary:BankName"],
                BankAddress = configuration["Beneficiary:BankAddress"],
                Swift = configuration["Beneficiary:Swift"],
                IntermediaryBank = configuration["Beneficiary:IntermediaryBank"],
                IntermediarySwift = configuration["Beneficiary:IntermediarySwift"],
                IsRead = false,
                IsPaid = false,
                CreatedAt = DateTime.UtcNow,
                Notes = createInvoiceDto.Notes,
                UserId = user.Id
            };

            return Created("/api/invoice/create", await invoiceService.CreateAsync(invoiceToCreate));
        }

        /// <summary>
        /// Updates an existing Invoice item.
        /// </summary>
        /// <returns>Status 200 and updated InvoiceDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /api/invoice/update
        ///     {
        ///        id: "516d8be0-3e18-4617-8781-86934b3333cd",
        ///        invoiceNo: "1",
        ///        invoiceTo: "Company 1",
        ///        item: "Price package Optimal",
        ///        quantity: "3",
        ///        price: "11.95",
        ///        total: "35.85",
        ///        beneficiary: "Beneficiary Name",
        ///        Account: "1111-2222-3333-4444",
        ///        beneficiaryEmail: "someone@somewhere.com",
        ///        bank: "Beneficiary Bank Name",
        ///        bankAddress: "Beneficiary Bank Address",
        ///        swift: "12345",
        ///        intermediaryBank: "Intermediary Bank",
        ///        intermediarySwift: "23456",
        ///        isRead: "false",
        ///        isPaid: "false",
        ///        notes: "Test notes",
        ///        userId: "e00875e2-e9f8-4e32-9db6-613e11aa7ff6",
        ///        createdAt: "11/05/2023"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated InvoiceDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the Invoice with given id not found</response>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] InvoiceDto invoiceDto) =>
            Ok(await invoiceService.UpdateAsync(invoiceDto));

        /// <summary>
        /// Partly updates an existing Invoice item.
        /// </summary>
        /// <param name="id">Identifier string id</param>
        /// <param name="patchDocument">Json Patch Document as array of operations</param>
        /// <returns>Status 200 and updated InvoiceDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH /api/invoice/partialInvoiceupdate/{id}
        ///     [
        ///         {
        ///             op: "replace",
        ///             path: "/notes",
        ///             value: "Updated Invoice notes"
        ///         }
        ///     ]
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated InvoiceDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the form with given id not found</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PartialUpdateAsync(string id, JsonPatchDocument<object> patchDocument) =>
            Ok(await invoiceService.PartialUpdateAsync(id, patchDocument));

        /// <summary>
        /// Deletes an Invoice Item.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/invoice/delete/33f84540-5e7c-474b-a030-77872d2738c6
        ///     
        /// </remarks>
        /// <param name="id">Identifier string id</param>
        /// <returns>Status 200</returns>
        /// <response code="200">Returns status 200</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id)
        {
            await invoiceService.DeleteAsync(id);
            return Ok();
        }
    }
}

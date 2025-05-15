using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Controllers.Image
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin, Carrier, Broker")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class ImageController(IImageService imageService, IUserService userService) : AppControllerBase
    {
        private readonly IImageService imageService = imageService;
        private readonly IUserService userService = userService;

        /// <summary>
        /// Creates Image item and saves IFormFile formFile, then updates user's account (increases current files size).
        /// For autherized users, to operate: profile avatar and logo, brand logo, form logo, question picture.
        /// </summary>
        /// <param name="formFile">IFormFile object from body</param>
        /// <returns>Status 201 and created ImageDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/image/upload/33f84540-5e7c-474b-a030-77872d2738c6
        ///     {
        ///        formFile: "IFormFile"
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the created ImageDto</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadAsync(IFormFile formFile)
        {
            var userId = (await userService.GetByEmailAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value)).Id;

            var createdImage = await imageService.CreateAsync(formFile, null, null, userId);

            return Created("/api/image/upload/images", createdImage);
        }

        /// <summary>
        /// Deletes an Image Item
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/image/delete?fileName=5d128cd7a124462086c8f0ab9e8b0b32.jpg
        ///     
        /// </remarks>
        /// <param name="fileName"></param>
        /// <returns>Status 200</returns>
        /// <response code="200">Returns status 200</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync([FromQuery] string fileName)
        {
            await imageService.DeleteAsync(fileName);

            return Ok();
        }
    }
}

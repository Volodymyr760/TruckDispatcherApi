using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TruckDispatcherApi.Library;
using TruckDispatcherApi.Models;
using TruckDispatcherApi.Services;

namespace TruckDispatcherApi.Controllers.Account
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Admin, Carrier, Broker")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class UserController(
        IEmailSender emailSender,
        IImageService imageService,
        IInvoiceService invoiceService,
        IMailTemplateService mailTemplateService,
        INotificationService notificationService,
        ITokenService tokenService,
        IUserService userService,
        RoleManager<IdentityRole> roleManager,
        SignInManager<User> signInManager,
        UserManager<User> userManager) : AppControllerBase
    {
        private readonly IEmailSender emailSender = emailSender;
        private readonly IImageService imageService = imageService;
        private readonly IInvoiceService invoiceService = invoiceService;
        private readonly IMailTemplateService mailTemplateService = mailTemplateService;
        private readonly INotificationService notificationService = notificationService;
        private readonly ITokenService tokenService = tokenService;
        private readonly IUserService userService = userService;

        private readonly RoleManager<IdentityRole> roleManager = roleManager;
        private readonly SignInManager<User> signInManager = signInManager;
        private readonly UserManager<User> userManager = userManager;

        /// <summary>
        /// Creates the new IdentityRole in db by given unique parameter.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/account/createRole/roleName
        ///
        /// Returns object like this:
        ///      {
        ///        id: "2194ad72-81db-410b-9bdd-d172732e3338",
        ///        name: "Test",
        ///        normalizedName: "TEST",
        ///        concurrencyStamp: "f8bf512d-5213-4f0a-a774-4f0f70547a7c"
        ///      }
        ///     
        /// </remarks>
        /// <param name="role">Role name</param>
        /// <returns>Created IdentityRole object</returns>
        /// <response code="201">Returns the created IdentityRole object</response>
        /// <response code="400">If the argument is not unique</response>
        /// <response code="401">If the user is not in Admin-role</response>
        [HttpGet("{role}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRoleAsync([FromRoute] string role)
        {
            var roleToSave = new IdentityRole { Name = role };
            if (!(await roleManager.CreateAsync(roleToSave)).Succeeded)
                return BadRequest(ResponseErrorFactory.GetBadRequestError($"Unable to add role {role}."));

            return Created("/api/account/createrole/{role}", roleToSave);
        }

        /// <summary>
        /// Replaces a user's email by new email. Sends new confirmation letter and removes existing tokens. 
        /// </summary>
        /// <returns>Status 200 and message</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/account/changeemail
        ///     {
        ///        existingEmail: "test@gmail.com",
        ///        newEmail: "test1@gmail.com",
        ///        password: "Password1."
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns status 200 and message</response>
        /// <response code="400">If the arguments are not valid: unique new email and correct password. Also while email server is not running.</response>
        /// <response code="401">If the user is not authorized</response>
        /// <response code="404">If the user not found</response>
        /// <response code="503">If the email service is unavailable</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> ChangeEmailAsync([FromBody] ChangeEmailDto changeEmailDto)
        {
            var user = await userManager.FindByEmailAsync(changeEmailDto.ExistingEmail);
            if (user == null) return NotFound(responseNotFoundError);
            if (await userManager.FindByEmailAsync(changeEmailDto.NewEmail) != null)
                return BadRequest(ResponseErrorFactory.GetBadRequestError("The email already in use.")); // is new email unique
            if (!ModelState.IsValid || !(await signInManager.CheckPasswordSignInAsync(user, changeEmailDto.Password, false)).Succeeded)
                return BadRequest(ResponseErrorFactory.GetBadRequestError("Invalid password."));
            await userManager.RemoveAuthenticationTokenAsync(user, "Truckdispatcher", TokenType.Refresh.ToString());
            var code = tokenService.GenerateRandomToken(30);
            var result = await userManager.SetEmailAsync(user, changeEmailDto.NewEmail);
            if (!result.Succeeded) // UserManager validates other user's fields while changing email
                return BadRequest(ResponseErrorFactory.GetBadRequestError(result.Errors.ToArray()[0].Description));
            await userManager.SetAuthenticationTokenAsync(user, "Truckdispatcher", TokenType.EmailConfirmation.ToString(), code);
            try
            {
                var mailTemplate = await mailTemplateService.GetByMailTemplateKey(MailTemplateKey.ChangeEmail);
                await emailSender.SendEmailHtmlWithTemplate($"{changeEmailDto.NewEmail}", mailTemplate,
                    [$"https://truckdispatcher.com/email-confirm/?code={code}&email={changeEmailDto.NewEmail}"]);
            }
            catch
            {
                return StatusCode(responseServiceUnavailableError.Status, responseServiceUnavailableError.Title);
            }

            return Ok("Email changed.");
        }

        /// <summary>
        /// Replaces a user's password by new password. Sends confirmation email. 
        /// </summary>
        /// <returns>Status 200 and message</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/account/changepassword
        ///     {
        ///        email: "test@gmail.com",
        ///        oldPassword: "Password1.",
        ///        newPassword: "NewPassword1."
        ///        confirmNewPassword: "NewPassword1."
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns status 200 and message</response>
        /// <response code="400">If the argument is not valid or some validation rules where broken</response>
        /// <response code="401">If the user is not authorized</response>
        /// <response code="404">If the user not found</response>
        /// <response code="503">If the email service is unavailable</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto changePasswordDto)
        {
            var user = await userManager.FindByEmailAsync(changePasswordDto.Email);
            if (user == null) return NotFound(responseNotFoundError);
            if (!ModelState.IsValid || !(await signInManager.CheckPasswordSignInAsync(user, changePasswordDto.OldPassword, false)).Succeeded)
                return BadRequest(ResponseErrorFactory.GetBadRequestError("Invalid password."));
            await userManager.RemoveAuthenticationTokenAsync(user, "Truckdispatcher", TokenType.Refresh.ToString());
            var result = await userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
            if (!result.Succeeded) return BadRequest(ResponseErrorFactory.GetBadRequestError(result.Errors.ToArray()[0].Description));
            try
            {
                var mailTemplate = await mailTemplateService.GetByMailTemplateKey(MailTemplateKey.ChangePassword);
                await emailSender.SendEmailHtmlWithTemplate($"{changePasswordDto.Email}", mailTemplate, []);
            }
            catch
            {
                return StatusCode(responseInternalServerError.Status, "Unable to change password.");
            }

            return Ok("Password changed.");
        }

        /// <summary>
        /// Changes user's roles. 
        /// </summary>
        /// <returns>Status 200 and list of actual user's roles</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/account/changeuserroles
        ///     {
        ///        userId: "808f4f76-6dd3-4183-9e26-5ac696b9327a",
        ///        neededRoles: ["Admin", "User"]
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns status 200 and list of actual user's roles</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="401">If the user is not authorized</response>
        /// <response code="404">If the user not found</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeUserRolesAsync([FromBody] ChangeRolesDto changeRolesDto)
        {
            var user = await userManager.FindByIdAsync(changeRolesDto.UserId);
            if (user == null) return NotFound(responseNotFoundError);

            foreach (var role in roleManager.Roles.Select(x => x.Name).ToList())
            {
                if (role == null) continue;
                if (changeRolesDto.UserRoles.Contains(role) && !(await userManager.IsInRoleAsync(user, role)))
                {
                    await userManager.AddToRoleAsync(user, role);
                }
                else if (!changeRolesDto.UserRoles.Contains(role) && (await userManager.IsInRoleAsync(user, role)))
                {
                    await userManager.RemoveFromRoleAsync(user, role);
                }
                else
                {
                    continue;
                }
            }
            await userManager.RemoveAuthenticationTokenAsync(user, "TruckDispatcherApi", TokenType.Refresh.ToString());
            // todo: send mail to user about changed roles, user will need to sign in

            return Ok();
        }

        /// <summary>
        /// Changes User.EmailConfirmed property
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/account/confirmemail?code=TntRy0cNM271AjzCXZ6Tz&amp;email=g@gmail.com
        /// 
        /// </remarks>
        /// <param name="code">Code from confirmation email</param>
        /// <param name="email">Users email</param>
        /// <returns>Ok("Email confirmed.")</returns>
        /// <response code="200">Returns the confirmation of success</response>
        /// <response code="400">If the code or email are wrong</response>
        /// <response code="404">If the user with given email is not found</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string code, string email)
        {
            if (code == null || email == null) return BadRequest(ResponseErrorFactory.GetBadRequestError("Wrong code or email."));
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return NotFound(responseNotFoundError);
            var result = await userManager.GetAuthenticationTokenAsync(user, "TruckDispatcherApi", TokenType.EmailConfirmation.ToString());
            if (result == null || result != code)
                return BadRequest(ResponseErrorFactory.GetBadRequestError("Unable to confirm email."));
            user.EmailConfirmed = true;
            await userManager.UpdateAsync(user);
            await userManager.RemoveAuthenticationTokenAsync(user, "TruckDispatcherApi", TokenType.EmailConfirmation.ToString());

            return Ok("Email confirmed.");
        }

        /// <summary>
        /// Deletes User and related data items.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /api/user/delete/33f84540-5e7c-474b-a030-77872d2738c6
        ///     
        /// </remarks>
        /// <param name="id">Account identifier string id</param>
        /// <returns>Status 200</returns>
        /// <response code="200">Returns status 200</response>
        /// <response code="400">If the user, which related to account stil has related contacts or forms</response>
        /// <response code="404">If the Account with given id not found</response>
        /// <response code="500">Internal Server Error</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id)
        {
            var userToDelete = await userManager.FindByIdAsync(id);
            if (userToDelete == null) return Ok();
            if (await userService.HasDriversAsync(userToDelete.Id))
                return BadRequest(ResponseErrorFactory.GetBadRequestError("User has related drivers, unable to remove the user."));
            if (await userService.HasLoadsAsync(userToDelete.Id))
                return BadRequest(ResponseErrorFactory.GetBadRequestError("User has related loads, unable to remove the user."));
            if (await userService.HasTrucksAsync(userToDelete.Id))
                return BadRequest(ResponseErrorFactory.GetBadRequestError("User has related trucks, unable to remove the user."));
            await userManager.RemoveAuthenticationTokenAsync(userToDelete, "DoFormApi", TokenType.Refresh.ToString());
            var roles = await userManager.GetRolesAsync(userToDelete);
            foreach (var role in roles) await userManager.RemoveFromRoleAsync(userToDelete, role);

            // remove all user's invoices
            var userInvoices = (await invoiceService.GetAsync(new SearchParams<InvoiceDto>()
            {
                PageSize = int.MaxValue,
                CurrentPage = 1,
                SearchCriteria = "",
                UserId = userToDelete.Id,
                SortField = "Invoice No",
                Order = OrderType.Ascending,
                IncludeNavProperties = false,
                ItemList = [],
                PageCount = 0,
                TotalItemsCount = 0
            })).ItemList;
            foreach (var invoice in userInvoices) if (invoice.Id != null) await invoiceService.DeleteAsync(invoice.Id);

            // remove all user's notifications
            var userNotifications = (await notificationService.GetAsync(new SearchParams<NotificationDto>()
            {
                PageSize = int.MaxValue,
                CurrentPage = 1,
                SearchCriteria = "",
                UserId = userToDelete.Id,
                SortField = "CreatedAt",
                Order = OrderType.Ascending,
                IncludeNavProperties = false,
                ItemList = [],
                PageCount = 0,
                TotalItemsCount = 0
            })).ItemList;
            foreach (var notification in userNotifications) if (notification.Id != null) await notificationService.DeleteAsync(notification.Id);

            // remove all user's images
            var userImages = (await imageService.GetAsync(new SearchParams<ImageDto>()
            {
                PageSize = int.MaxValue,
                CurrentPage = 1,
                SearchCriteria = "",
                UserId = userToDelete.Id,
                SortField = "",
                Order = OrderType.None,
                IncludeNavProperties = false,
                ItemList = [],
                PageCount = 0,
                TotalItemsCount = 0
            })).ItemList;
            foreach (var image in userImages) await imageService.DeleteAsync(image.FileName);

            await userService.DeleteAsync(id);

            await userManager.DeleteAsync(userToDelete);
            try
            {
                var mailTemplate = await mailTemplateService.GetByMailTemplateKey(MailTemplateKey.UserAccountDeleted);
                await emailSender.SendEmailHtmlWithTemplate($"{userToDelete.Email}", mailTemplate,
                    ["https://doyourform.com"]);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Gets a specified UserDto item.
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Status OK and UserDto item</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/user/get/?id=516d8be0-3e18-4617-8781-86934b3333cd
        ///     
        /// </remarks>
        /// <response code="200">Returns the requested UserDto item</response>
        /// <response code="404">If the User with given id not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] string id) =>
            Ok(await userService.GetAsync(id));

        /// <summary>
        /// Gets all existing user's roles from db. 
        /// </summary>
        /// <returns>Status 200 and ChangeRolesDto</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/account/getroles/33f84540-5e7c-474b-a030-77872d2738c6
        ///     
        /// </remarks>
        /// <response code="200">Returns status 200 and ChangeRolesDto</response>
        /// <response code="404">If the user not found</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoles([FromRoute] string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return BadRequest(responseBadRequestError);
            var changeRolesDto = new ChangeRolesDto()
            {
                UserId = id,
                UserRoles = await userManager.GetRolesAsync(user),
                AllRoles = [.. roleManager.Roles]
            };

            return Ok(changeRolesDto);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GetSearchSettinsAsync([FromRoute] string id) =>
            Ok(await userService.GetSearchSettingsAsync(id));

        /// <summary>
        /// Creates PasswordReset token and sends the email to user's email-address.
        /// </summary>
        /// <returns>Status 200 and the given by user email-address</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/account/forgotpassword?email=test@gmail.com
        ///     
        /// </remarks>
        /// <param name="email">The email given by user to send the created token as code.</param>
        /// <response code="200">Ok status and the email given by user.</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="503">If the email service is unavailable.</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> ForgotPasswordAsync([FromQuery] string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return NotFound(ResponseErrorFactory.GetNotFoundError("User not found."));

            var code = tokenService.GenerateRandomToken(30);
            await userManager.SetAuthenticationTokenAsync(user, "Truckdispatcher", TokenType.ResetPassword.ToString(), code);
            try
            {
                var mailTemplate = await mailTemplateService.GetByMailTemplateKey(MailTemplateKey.ForgotPassword);
                await emailSender.SendEmailHtmlWithTemplate(email, mailTemplate,
                    [$"https://truckdispatcher.com/reset-password?code={code}&email={email}"]);
            }
            catch
            {
                return StatusCode(responseServiceUnavailableError.Status, responseServiceUnavailableError.Title);
            }

            return Ok(email);
        }

        /// <summary>
        /// Creates new access and refresh tokens.
        /// User can use different browsers and when user is logged in once - possible to send existing tokens:
        /// if refresh-token exists and !expired: yes - send existing tokens, no - create a new tokens pair.
        /// </summary>
        /// <returns>Status 200 and created access and refresh tokens</returns>
        /// <remarks>
        /// Sample request:
        ///     
        ///     POST /api/account/login
        ///     {
        ///         email: "logisticmaster.2000@gmail.com",
        ///         password: "Password1."
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns the newly created access / refresh tokens</response>
        /// <response code="400">If the arguments / password are wrong or email is not confirmed</response>
        /// <response code="404">If the user is not found</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginUserDto loginUserDto)
        {
            var user = await userManager.FindByEmailAsync(loginUserDto.Email);
            if (user == null) return NotFound(ResponseErrorFactory.GetNotFoundError("Unable to sign in with given credentials."));
            if (!user.EmailConfirmed) return BadRequest(ResponseErrorFactory.GetBadRequestError("Please confirm your email first."));
            if (!(await signInManager.CheckPasswordSignInAsync(user, loginUserDto.Password, false)).Succeeded)
                return BadRequest(ResponseErrorFactory.GetBadRequestError("Invalid password."));

            var refreshToken = await userManager.GetAuthenticationTokenAsync(user, "TruckDispatcherApi", TokenType.Refresh.ToString());
            if (string.IsNullOrEmpty(refreshToken) || tokenService.IsTokenExpired(refreshToken))
            {
                refreshToken = await GenerateTokenAsync(user, TokenType.Refresh, 60 * 24 * 35);
                await userManager.SetAuthenticationTokenAsync(user, "TruckDispatcherApi", TokenType.Refresh.ToString(), refreshToken);
                userService.SetCashedUserRefreshToken(refreshToken);
            }

            await userService.UpdateLastLoginDateAsync(user.Id);

            var authModel = new AuthModel()
            {
                User = userService.GetApplicationUserDto(user),
                Roles = await userManager.GetRolesAsync(user),
                Tokens = new TokenModel() { AccessToken = await GenerateTokenAsync(user, TokenType.Access, 60 * 24 * 30), RefreshToken = refreshToken }
            };

            return Ok(authModel);
        }

        /// <summary>
        /// Makes user logged out - deletes access and refresh tokens from db
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/account/logout/john@gmail.com
        ///     
        /// </remarks>
        /// <param name="email">Users email</param>
        /// <returns>Ok("User logged out.")</returns>
        /// <response code="200">Returns the confirmation of success</response>
        /// <response code="404">If the user with given email not found</response>
        [HttpGet("{email}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LogoutAsync([FromRoute] string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return NotFound(responseNotFoundError);
            await userManager.RemoveAuthenticationTokenAsync(user, "TruckDispatcherApi", TokenType.Refresh.ToString());

            return Ok("User is logged out.");
        }


        /// <summary>
        /// Partly updates an existing User item.
        /// </summary>
        /// <param name="id">Identifier string id</param>
        /// <param name="patchDocument">Json Patch Document as array of operations</param>
        /// <returns>Status 200 and updated UserDto object</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     PATCH /api/user/partialUserUpdate/74d58e64-cfce-4d28-9df7-9aee4a7cc9b8
        ///     [
        ///         {
        ///             op: "replace",
        ///             path: "/searchDeadheads",
        ///             value: 250
        ///         }
        ///     ]
        ///     
        /// </remarks>
        /// <response code="200">Returns the updated UserDto item</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the form with given id not found</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PartialUpdateAsync([FromRoute] string id, JsonPatchDocument<object> patchDocument) =>
            Ok(await userService.PartialUpdateAsync(id, patchDocument));

        /// <summary>
        /// Creates a new User.
        /// </summary>
        /// <returns>Status 201 and created User.Id</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/account/register
        ///     {
        ///        companyName: "Carrier LLC",
        ///        role: 2,
        ///        firstName: "John",
        ///        lastName: "Snow",
        ///        phoneNumber: "+1123456789",
        ///        email: "john@gmail.com",
        ///        password: "Password1.",
        ///        confirmPassword: "Password1."
        ///     }
        ///     
        /// </remarks>
        /// <param name="registerUserDto">Instance of type RegisterUserDto</param>
        /// <response code="201">Returns the newly created User.Id</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="503">If the email service is unavailable</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserDto registerUserDto)
        {
            var user = new User
            {
                UserName = registerUserDto.FirstName + registerUserDto.LastName,
                Email = registerUserDto.Email,
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                CompanyName = registerUserDto.CompanyName,
                PhoneNumberConfirmed = true,
                StartPayedPeriodDate = DateTime.UtcNow,
                FinishPayedPeriodDate = DateTime.UtcNow.AddDays(7),
                LastLoginDate = DateTime.UtcNow,
                AccountStatus = AccountStatus.ActiveUser,
                SearchDeadheads = 150,
                SearchMilesMin = 200,
                SearchMilesMax = 3000,
                SearchSortField = "Profit Per Mile",
                SearchSort = OrderType.Descending
            };

            if (!ModelState.IsValid || !(await userManager.CreateAsync(user, registerUserDto.Password)).Succeeded)
                return BadRequest(ResponseErrorFactory.GetBadRequestError("Unable to register user with the specified credentials."));

            if (registerUserDto.Role == AppRoles.Broker) await userManager.AddToRolesAsync(user, [nameof(AppRoles.Broker)]);
            if (registerUserDto.Role == AppRoles.Carrier) await userManager.AddToRolesAsync(user, [nameof(AppRoles.Carrier)]);

            var code = tokenService.GenerateRandomToken(30);

            try
            {
                var mailTemplate = await mailTemplateService.GetByMailTemplateKey(MailTemplateKey.ConfirmEmail);
                await Task.WhenAll(
                    userManager.SetAuthenticationTokenAsync(user, "TruckDispatcherApi", TokenType.EmailConfirmation.ToString(), code)
                    //,
                    //emailSender.SendEmailHtmlWithTemplate($"{user.Email}", mailTemplate,
                    //[$"https://doyourform.com/email-confirm/?code={code}&email={user.Email}"])
                    );
                await notificationService.NotifyAdmins(NotificationFactory.GetNotification(
                    null, "TruckDispatcherApi Registration", $"New user {user.FirstName} {user.LastName} has been registered. Id: {user.Id}", "/admin/users", "", ""));
                await userManager.SetAuthenticationTokenAsync(user, "TruckDispatcherApi", TokenType.EmailConfirmation.ToString(), code);
            }
            catch
            {
                return StatusCode(responseServiceUnavailableError.Status, responseServiceUnavailableError.Title);
            }

            return Created("api/account/register", code);
        }

        /// <summary>
        /// Refreshes access and refresh tokens by previous values.
        /// </summary>
        /// <returns>Status 201 and created access and refresh tokens</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/account/refreshtoken
        ///     {
        ///        accessToken: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWI...",
        ///        refreshToken: "PFYIdnb0vMvz0FcV/jKKgbpT3MEA0FNGAPegBkWXr00=",
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns the newly created tokens</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="404">If the user with given email not found</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshTokenAsync(TokenModel tokenModel)
        {
            try
            {
                if (tokenService.IsTokenExpired(tokenModel.RefreshToken)) throw new Exception();
                var userEmail = tokenService.GetUserEmailFromExpiredToken(tokenModel.RefreshToken);
                var userDto = await userService.GetByEmailAsync(userEmail) ?? throw new Exception("User not found");
                var user = userService.GetApplicationUser(userDto);
                var refreshToken = userService.GetCashedUserRefreshToken(tokenModel.RefreshToken);
                if (string.IsNullOrEmpty(refreshToken))
                {
                    refreshToken = await userManager.GetAuthenticationTokenAsync(user, "Truckdispatcher", TokenType.Refresh.ToString());
                    if (string.IsNullOrEmpty(refreshToken)) throw new Exception();
                    if (tokenModel.RefreshToken != refreshToken)// here is critical to check Email from token, saved in db
                    {
                        var refreshTokenFromDbHasEmail = tokenService.GetUserEmailFromExpiredToken(refreshToken);
                        if (refreshTokenFromDbHasEmail != userEmail) throw new Exception();
                    }
                    userService.SetCashedUserRefreshToken(refreshToken);
                    await userService.UpdateLastLoginDateAsync(user.Id);
                }
                var authModel = new AuthModel()
                {
                    User = userDto,
                    Roles = await userManager.GetRolesAsync(user),
                    Tokens = new TokenModel() { AccessToken = await GenerateTokenAsync(user, TokenType.Access, 60 * 24 * 30), RefreshToken = refreshToken }
                };

                return Created("api/account/refreshToken", authModel);
            }
            catch
            {
                return BadRequest(ResponseErrorFactory.GetBadRequestError("Unable to refresh token."));// so, user should log in
            }
        }

        /// <summary>
        /// Resets user's password.
        /// </summary>
        /// <returns>Status 200 and success message.</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/account/resetpassword
        ///     {
        ///         code: "SomeResetPasswordToken",
        ///         email: "test@gmail.com",
        ///         password: "Password1.",
        ///         confirmPassword: "Password1."
        ///     }
        ///     
        /// </remarks>
        /// <param name="resetPasswordDto">ResetPasswordDto object</param>
        /// <response code="200">Ok status and success message.</response>
        /// <response code="400">If the code (token) is not correct or another error of UserManager validation happened.</response>
        /// <response code="404">If the user is not found</response>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null) return NotFound(responseNotFoundError);

            var token = await userManager.GetAuthenticationTokenAsync(user, "TruckDispatcherApi", TokenType.ResetPassword.ToString());
            if (token == null || token != resetPasswordDto.Code)
                return BadRequest(ResponseErrorFactory.GetBadRequestError("Unable to reset password."));
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, resetPasswordDto.Password);
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(ResponseErrorFactory.GetBadRequestError(result.Errors.ToArray()[0].Description));
            await userManager.RemoveAuthenticationTokenAsync(user, "TruckDispatcherApi", TokenType.ResetPassword.ToString());
            await userManager.RemoveAuthenticationTokenAsync(user, "TruckDispatcherApi", TokenType.Refresh.ToString());

            return Ok("Password changed.");
        }

        /// <summary>
        /// Searchs list of UserDto
        /// </summary>
        /// <param name="userSearchParams"></param>
        /// <returns>Status 200 and list of UserDto</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/user/search
        ///     {
        ///        currentPage: 1,
        ///        pageSize: 100,
        ///        searchCriteria: "",
        ///        userId: "", // empty string
        ///        sortField: "First Name",
        ///        order: 0,
        ///        includeNavProperties: 1,
        ///        itemList: [],
        ///        accountStatus: 0
        ///     }
        ///     
        /// </remarks> 
        /// <response code="200">Returns list of UserDto</response>
        /// <response code="400">If the argument is not valid or something went wrong</response>
        [HttpPost]
        public async Task<IActionResult> SearchAsync([FromBody] UserSearchParams userSearchParams) =>
            Ok(await userService.GetAsync(userSearchParams));

        /// <summary>
        /// Updates an existing user fields: PhoneNumber, FirstName, LastName, AvatarUrl,
        /// StartPayedPeriodDate, FinishPayedPeriodDate and AccountStatus
        /// Available for User to change their profile settings
        /// </summary>
        /// <returns>Status 200 and updated UserDto</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/user/update
        ///     {
        ///        id: "92c79472-9da6-4da3-a052-358f465d8864",
        ///        firstName: "John",
        ///        lastName: "Snow",
        ///        avatar: "https://somewhere.com/?userphoto=asdasdas",
        ///        accountStatus: 0,
        ///        email: "john@gmail.com",
        ///        emailConfirmed: "true",
        ///        phoneNumber: "+380961111111",
        ///        startPayedPeriodDate: "2025-02-08T00:00:00Z",
        ///        finishPayedPeriodDate: "2025-02-28T00:00:00Z",
        ///        createdAt: "2024-11-25T00:00:00Z",
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns status 200 and updated ApplicationUserDto</response>
        /// <response code="400">If the argument is not valid</response>
        /// <response code="401">If the user is not authorized</response>
        /// <response code="404">If the user not found</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] UserDto userDto)
        {
            if (string.IsNullOrEmpty(userDto.Id)) return BadRequest(responseBadRequestError);
            var user = await userManager.FindByIdAsync(userDto.Id);
            if (user == null) return BadRequest(responseBadRequestError);
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.PhoneNumber = userDto.PhoneNumber;
            user.Avatar = userDto.Avatar;
            user.StartPayedPeriodDate = userDto.StartPayedPeriodDate;
            user.FinishPayedPeriodDate = userDto.FinishPayedPeriodDate;
            user.AccountStatus = userDto.AccountStatus;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(ResponseErrorFactory.GetBadRequestError(result.Errors.ToArray()[0].Description));
            userService.ClearCache(userDto.Email);
            userService.ClearCache(userDto.Id);

            return Ok(userDto);
        }

        private async Task<string> GenerateTokenAsync(User user, TokenType tokenType, double period)
        {
            if (user.Email == null) throw new ArgumentNullException(user.Email); //IdentityUser<TKey>.Email can be null
            var tokenClaims = new List<Claim> { new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) };
            if (tokenType == TokenType.Access)
            {
                var roles = await userManager.GetRolesAsync(user);
                foreach (var role in roles) tokenClaims.Add(new Claim("role", role));
            }

            return tokenService.GenerateAccessToken(tokenClaims, period);
        }
    }
}

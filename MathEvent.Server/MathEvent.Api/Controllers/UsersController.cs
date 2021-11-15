using AutoMapper;
using MathEvent.AuthorizationHandlers;
using MathEvent.Contracts.Services;
using MathEvent.Contracts.Validators;
using MathEvent.DTOs.Users;
using MathEvent.Models.Others;
using MathEvent.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly IUserService _userService;

        private readonly IAuthorizationService _authorizationService;

        private readonly IUserCreateModelValidator _userCreateModelValidator;

        private readonly IUserUpdateModelValidator _userUpdateModelValidator;

        private readonly IForgotPasswordModelValidator _forgotPasswordModelValidator;

        private readonly IForgotPasswordResetModelValidator _forgotPasswordResetModelValidator;

        public UsersController(
            IMapper mapper,
            IUserService userService,
            IAuthorizationService authorizationService,
            IUserCreateModelValidator userCreateModelValidator,
            IUserUpdateModelValidator userUpdateModelValidator,
            IForgotPasswordModelValidator forgotPasswordModelValidator,
            IForgotPasswordResetModelValidator forgotPasswordResetModelValidator)
        {
            _mapper = mapper;
            _userService = userService;
            _authorizationService = authorizationService;
            _userCreateModelValidator = userCreateModelValidator;
            _userUpdateModelValidator = userUpdateModelValidator;
            _forgotPasswordModelValidator = forgotPasswordModelValidator;
            _forgotPasswordResetModelValidator = forgotPasswordResetModelValidator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UserReadModel>>> List([FromQuery] IDictionary<string, string> filters)
        {
            var users = await _userService.ListAsync(filters);

            return Ok(users);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserWithEventsReadModel>> Retrieve([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest($"id не задан");
            }

            var user = await _userService.RetrieveAsync(id);

            if (user is null)
            {
                return NotFound($"Пользователь с id={id} не найден");
            }

            return Ok(user);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Create([FromBody] UserCreateModel userCreateModel)
        {
            var validationResult = await _userCreateModelValidator.Validate(userCreateModel);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var createdUser = await _userService.CreateAsync(userCreateModel);

            if (createdUser is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время создания пользователя");
            }

            return CreatedAtAction(nameof(Retrieve), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromRoute] string id, [FromBody] UserUpdateModel userUpdateModel)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("id не задан");
            }

            var validationResult = await _userUpdateModelValidator.Validate(userUpdateModel);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var user = await _userService.RetrieveAsync(id);

            if (user is null)
            {
                return NotFound($"Пользователь с id={id} не найден");
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, user, Operations.Update);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя редактировать пользователя с id={id}");
            }

            var updatedUser = await _userService.UpdateAsync(id, userUpdateModel);

            if (updatedUser is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время обновления пользователя");
            }

            return Ok(updatedUser);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialUpdate([FromRoute] string id, [FromBody] JsonPatchDocument<UserUpdateModel> patchDocument)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("id не задан");
            }

            if (patchDocument is null)
            {
                return BadRequest("Тело запроса не задано");
            }

            var user = await _userService.RetrieveAsync(id);

            if (user is null)
            {
                return NotFound($"Пользователь с id={id} не найден");
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, user, Operations.Update);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя редактировать пользователя с id={id}");
            };

            var userDTO = _mapper.Map<UserWithEventsDTO>(user);
            var userToPatch = _mapper.Map<UserUpdateModel>(userDTO);
            patchDocument.ApplyTo(userToPatch, ModelState);

            var validationResult = await _userUpdateModelValidator.Validate(userToPatch);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var updatedUser = await _userService.UpdateAsync(id, userToPatch);

            if (updatedUser is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время обновления пользователя");
            }

            return Ok(updatedUser);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Destroy([FromRoute] string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("id не задан");
            }

            var user = await _userService.RetrieveAsync(id);

            if (user is null)
            {
                return NotFound($"Пользователь с id={id} не найден");
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, user, Operations.Delete);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя удалять пользователя с id={id}");
            }

            await _userService.DeleteAsync(id);

            return NoContent();
        }

        [HttpPost("ForgotPassword/")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordModel forgotPasswordModel)
        {
            var validationResult = await _forgotPasswordModelValidator.Validate(forgotPasswordModel);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            await _userService.ForgotPasswordAsync(forgotPasswordModel.Email);

            return Ok();
        }

        [HttpPost("ResetPassword/")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPasswordAsync(ForgotPasswordResetModel resetModel)
        {
            var validationResult = await _forgotPasswordResetModelValidator.Validate(resetModel);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            await _userService.ResetPasswordAsync(resetModel);

            return Ok();
        }

        [HttpGet("Statistics/")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ChartData>>> Statistics([FromQuery] IDictionary<string, string> filters)
        {
            var usersStatistics = await _userService.GetUsersStatistics(filters);

            return Ok(usersStatistics);
        }

        [HttpGet("Statistics/{id}")]
        public async Task<ActionResult<IEnumerable<ChartData>>> UserStatistics([FromRoute] string id)
        {
            var userStatistics = await _userService.GetUserStatistics(id);

            return Ok(userStatistics);
        }
    }
}

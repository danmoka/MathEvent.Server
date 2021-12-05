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
using System;
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

        public UsersController(
            IMapper mapper,
            IUserService userService,
            IAuthorizationService authorizationService,
            IUserCreateModelValidator userCreateModelValidator,
            IUserUpdateModelValidator userUpdateModelValidator)
        {
            _mapper = mapper;
            _userService = userService;
            _authorizationService = authorizationService;
            _userCreateModelValidator = userCreateModelValidator;
            _userUpdateModelValidator = userUpdateModelValidator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UserReadModel>>> List([FromQuery] IDictionary<string, string> filters)
        {
            var users = await _userService.List(filters);

            return Ok(users);
        }

        [HttpGet("{identityUserId}")]
        public async Task<ActionResult<UserWithEventsReadModel>> Retrieve([FromRoute] Guid identityUserId)
        {
            if (Guid.Empty == identityUserId)
            {
                return BadRequest($"identityUserId не задан");
            }

            var user = await _userService.RetrieveByIdentityUserId(identityUserId);

            if (user is null)
            {
                return NotFound($"Пользователь с id={identityUserId} платформы аутентификации не найден");
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrUpdate([FromBody] UserCreateModel userCreateModel)
        {
            var user = await _userService.GetUserByEmail(userCreateModel.Email);

            if (user is not null)
            {
                var userDTO = _mapper.Map<UserWithEventsDTO>(user);
                var updateModel = _mapper.Map<UserUpdateModel>(userDTO);

                updateModel.IdentityUserId = userCreateModel.IdentityUserId;
                updateModel.Name = userCreateModel.Name;
                updateModel.Surname = userCreateModel.Surname;

                var validationResult = await _userUpdateModelValidator.Validate(updateModel);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, user, Operations.Update);

                if (!authorizationResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя редактировать пользователя");
                }

                user = await _userService.UpdateByEmail(user.Email, updateModel);
            }
            else
            {
                var validationResult = await _userCreateModelValidator.Validate(userCreateModel);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, userCreateModel, Operations.Create);

                if (!authorizationResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя создать пользователя");
                }

                user = await _userService.Create(userCreateModel);
            }

            if (user is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время создания пользователя");
            }

            return CreatedAtAction(nameof(Retrieve), new { identityUserId = user.IdentityUserId }, user);
        }

        [HttpPut("{identityUserId}")]
        public async Task<ActionResult> Update([FromRoute] Guid identityUserId, [FromBody] UserUpdateModel userUpdateModel)
        {
            if (Guid.Empty == identityUserId)
            {
                return BadRequest("identityId не задан");
            }

            var validationResult = await _userUpdateModelValidator.Validate(userUpdateModel);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var user = await _userService.RetrieveByIdentityUserId(identityUserId);

            if (user is null)
            {
                return NotFound($"Пользователь с id={identityUserId} платформы аутентификации не найден");
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, user, Operations.Update);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя редактировать пользователя с id={identityUserId} платформы аутентификации");
            }

            var updatedUser = await _userService.UpdateByIdentityUserId(identityUserId, userUpdateModel);

            if (updatedUser is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время обновления пользователя");
            }

            return Ok(updatedUser);
        }

        [HttpPatch("{identityUserId}")]
        public async Task<ActionResult> PartialUpdate([FromRoute] Guid identityUserId, [FromBody] JsonPatchDocument<UserUpdateModel> patchDocument)
        {
            if (Guid.Empty == identityUserId)
            {
                return BadRequest("identityUserId не задан");
            }

            if (patchDocument is null)
            {
                return BadRequest("Тело запроса не задано");
            }

            var user = await _userService.RetrieveByIdentityUserId(identityUserId);

            if (user is null)
            {
                return NotFound($"Пользователь с id={identityUserId} платформы аутентификации не найден");
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, user, Operations.Update);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя редактировать пользователя с id={identityUserId} платформы аутентификации");
            };

            var userDTO = _mapper.Map<UserWithEventsDTO>(user);
            var userToPatch = _mapper.Map<UserUpdateModel>(userDTO);
            patchDocument.ApplyTo(userToPatch, ModelState);

            var validationResult = await _userUpdateModelValidator.Validate(userToPatch);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var updatedUser = await _userService.UpdateByIdentityUserId(identityUserId, userToPatch);

            if (updatedUser is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время обновления пользователя");
            }

            return Ok(updatedUser);
        }

        [HttpDelete("{identityUserId}")]
        public async Task<ActionResult> Destroy([FromRoute] Guid identityUserId)
        {
            if (Guid.Empty == identityUserId)
            {
                return BadRequest("id не задан");
            }

            var user = await _userService.RetrieveByIdentityUserId(identityUserId);

            if (user is null)
            {
                return NotFound($"Пользователь с id={identityUserId} платформы аутентификации не найден");
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, user, Operations.Delete);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя удалять пользователя с id={identityUserId} платформы аутентификации");
            }

            await _userService.DeleteByIdentityUserId(identityUserId);

            return NoContent();
        }

        [HttpGet("Statistics/")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ChartData>>> Statistics([FromQuery] IDictionary<string, string> filters)
        {
            var usersStatistics = await _userService.GetUsersStatistics(filters);

            return Ok(usersStatistics);
        }

        [HttpGet("Statistics/{identityUserId}")]
        public async Task<ActionResult<IEnumerable<ChartData>>> UserStatistics([FromRoute] Guid identityUserId)
        {
            if (Guid.Empty == identityUserId)
            {
                return BadRequest("id не задан");
            }

            var userStatistics = await _userService.GetUserStatistics(identityUserId);

            return Ok(userStatistics);
        }
    }
}

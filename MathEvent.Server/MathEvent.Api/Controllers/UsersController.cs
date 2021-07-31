using AutoMapper;
using MathEvent.Converters.Identities.DTOs;
using MathEvent.Converters.Identities.Models;
using MathEvent.Converters.Others;
using MathEvent.Services.Services;
using Microsoft.AspNetCore.Authorization;
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

        private readonly UserService _userService;

        public UsersController(IMapper mapper, UserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        // GET api/Users/?key1=value1&key2=value2
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UserReadModel>>> ListAsync([FromQuery] IDictionary<string, string> filters)
        {
            var userResult = await _userService.ListAsync(filters);

            if (userResult.Succeeded)
            {
                var userReadModels = userResult.Entity;

                if (userReadModels is not null)
                {
                    return Ok(userReadModels);
                }
            }

            return NotFound(userResult.Messages);
        }

        // GET api/Users/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserWithEventsReadModel>> RetrieveAsync(string id)
        {
            if (id is null)
            {
                return BadRequest();
            }

            var userResult = await _userService.RetrieveAsync(id);

            if (userResult.Succeeded)
            {
                var userReadModel = userResult.Entity;

                if (userReadModel is not null)
                {
                    return Ok(userReadModel);
                }
            }

            return NotFound(userResult.Messages);
        }

        // POST api/Users
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CreateAsync([FromBody] UserCreateModel userCreateModel)
        {
            if (!TryValidateModel(userCreateModel))
            {
                return ValidationProblem(ModelState);
            }

            var createResult = await _userService.CreateAsync(userCreateModel);

            if (createResult.Succeeded)
            {
                var createdUser = createResult.Entity;

                if (createdUser is null)
                {
                    return StatusCode(201);
                }

                return StatusCode(201, createdUser);
            }
            else
            {
                return BadRequest(createResult.Messages);
            }
        }

        // PUT api/Users/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(string id, [FromBody] UserUpdateModel userUpdateModel)
        {
            if (id is null)
            {
                return BadRequest();
            }

            var userResult = await _userService.GetUserEntityAsync(id);

            if (!userResult.Succeeded)
            {
                return NotFound(userResult.Messages);
            }

            if (!TryValidateModel(userUpdateModel))
            {
                return ValidationProblem(ModelState);
            }

            var updateResult = await _userService.UpdateAsync(id, userUpdateModel);

            if (updateResult.Succeeded)
            {
                var updatedUser = updateResult.Entity;

                if (updatedUser is null)
                {
                    return Ok(id);
                }

                return Ok(updatedUser);
            }
            else
            {
                return BadRequest(updateResult.Messages);
            }
        }

        // PATCH api/Users/{id}
        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialupdateAsync(string id, [FromBody] JsonPatchDocument<UserUpdateModel> patchDocument)
        {
            if (id is null)
            {
                return BadRequest();
            }

            if (patchDocument is null)
            {
                return BadRequest();
            }

            var userResult = await _userService.GetUserEntityAsync(id);

            if (!userResult.Succeeded)
            {
                return NotFound(userResult.Messages);
            }

            var userEntity = userResult.Entity;

            if (userEntity is null)
            {
                return NotFound();
            }

            var userDTO = _mapper.Map<UserWithEventsDTO>(userEntity);
            var userToPatch = _mapper.Map<UserUpdateModel>(userDTO);
            patchDocument.ApplyTo(userToPatch, ModelState);

            if (!TryValidateModel(userToPatch))
            {
                return ValidationProblem(ModelState);
            }

            var updateResult = await _userService.UpdateAsync(id, userToPatch);

            if (updateResult.Succeeded)
            {
                var updatedUser = updateResult.Entity;

                if (updatedUser is null)
                {
                    return Ok(id);
                }

                return Ok(updatedUser);
            }
            else
            {
                return BadRequest(updateResult.Messages);
            }
        }

        // DELETE api/Users/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DestroyAsync(string id)
        {
            if (id is null)
            {
                return BadRequest();
            }

            var userResult = await _userService.GetUserEntityAsync(id);

            if (!userResult.Succeeded)
            {
                return NotFound(userResult.Messages);
            }

            var deleteResult = await _userService.DeleteAsync(id);

            if (deleteResult.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(deleteResult.Messages);
            }
        }

        // GET api/Users/Statistics/?key1=value1&key2=value2
        [HttpGet("Statistics/")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SimpleStatistics>>> StatisticsAsync([FromQuery] IDictionary<string, string> filters)
        {
            var userResult = await _userService.GetSimpleStatistics(filters);

            if (userResult.Succeeded)
            {
                return Ok(userResult.Entity);
            }

            return NotFound(userResult.Messages);
        }

        // GET api/Users/Statistics/{id}
        [HttpGet("Statistics/{id}")]
        public async Task<ActionResult<IEnumerable<SimpleStatistics>>> StatisticsAsync(string id)
        {
            var userResult = await _userService.GetUserStatistics(id);

            if (userResult.Succeeded)
            {
                return Ok(userResult.Entity);
            }

            return NotFound(userResult.Messages);
        }
    }
}

using AutoMapper;
using MathEvent.Contracts;
using MathEvent.Converters.Identities.DTOs;
using MathEvent.Entities.Models.Identities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathEvent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IMapper _mapper;

        public UsersController(IRepositoryWrapper wrapper, IMapper mapper)
        {
            _repositoryWrapper = wrapper;
            _mapper = mapper;
        }

        // GET api/Users
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UserReadDTO>>> ListAsync()
        {
            var userModels = await _repositoryWrapper.User
                .FindAll()
                .ToListAsync();

            if (userModels is not null)
            {
                return Ok(_mapper.Map<IEnumerable<UserReadDTO>>(userModels));
            }

            return NotFound();
        }

        // GET api/Users/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserWithEventsReadDTO>> RetriveAsync(string id)
        {
            var userModel = await _repositoryWrapper.User
                .FindByCondition(user => user.Id == id)
                .Include(user => user.Events)
                .SingleOrDefaultAsync();

            if (userModel is not null)
            {
                return Ok(_mapper.Map<UserWithEventsReadDTO>(userModel));
            }

            return NotFound();
        }

        // POST api/Users
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CreateAsync([FromBody] UserCreateDTO userCreateDTO)
        {
            var userModel = _mapper.Map<ApplicationUser>(userCreateDTO);

            if (userModel is not null)
            {
                var createResult = await _repositoryWrapper.User
                    .CreateAsync(userModel, userCreateDTO.Password);
                // TODO: AddToRoleAsync

                if (createResult.Succeeded)
                {
                    await _repositoryWrapper.SaveAsync();

                    return Ok();
                }
                else
                {
                    return BadRequest(createResult.Errors);
                }
            }

            return BadRequest();
        }

        // PUT api/Users/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(string id, [FromBody] UserUpdateDTO userUpdateDTO)
        {
            var userModel = await _repositoryWrapper.User
                .FindByCondition(user => user.Id == id)
                .Include(user => user.Events)
                .SingleOrDefaultAsync();
            // TODO: ? AddToRoleAsync

            if (userModel is not null)
            {
                _mapper.Map(userUpdateDTO, userModel);

                if (!TryValidateModel(userModel))
                {
                    return ValidationProblem(ModelState);
                }

                var updateResult = await _repositoryWrapper.User
                    .UpdateAsync(userModel);

                if (updateResult.Succeeded)
                {
                    await _repositoryWrapper.SaveAsync();

                    return Ok();
                }
                else
                {
                    return BadRequest(updateResult.Errors);
                }
            }

            return NotFound();
        }

        // PATCH api/Users/{id}
        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialupdateAsync(string id, [FromBody] JsonPatchDocument<UserUpdateDTO> patchDocument)
        {
            var userModel = await _repositoryWrapper.User
                .FindByCondition(user => user.Id == id)
                .Include(user => user.Events)
                .SingleOrDefaultAsync();
            // TODO: ? AddToRoleAsync

            if (userModel is not null)
            {
                var userToPatch = _mapper.Map<UserUpdateDTO>(userModel);
                patchDocument.ApplyTo(userToPatch, ModelState);

                if (!TryValidateModel(userToPatch))
                {
                    return ValidationProblem(ModelState);
                }

                _mapper.Map(userToPatch, userModel);

                var updateResult = await _repositoryWrapper.User
                    .UpdateAsync(userModel);

                if (updateResult.Succeeded)
                {
                    await _repositoryWrapper.SaveAsync();

                    return Ok();
                }
                else
                {
                    return BadRequest(updateResult.Errors);
                }
            }

            return NotFound();
        }

        // DELETE api/Users/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DestroyAsync(string id)
        {
            var userModel = await _repositoryWrapper.User
                .FindByCondition(user => user.Id == id)
                .SingleOrDefaultAsync();

            if (userModel is not null)
            {
                var deleteResult = await _repositoryWrapper.User
                    .DeleteAsync(userModel);

                if (deleteResult.Succeeded)
                {
                    await _repositoryWrapper.SaveAsync();

                    return NoContent();
                }
                else
                {
                    return BadRequest(deleteResult.Errors);
                }
            }

            return NotFound();
        }
    }
}

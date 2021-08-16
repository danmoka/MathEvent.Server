using AutoMapper;
using MathEvent.AuthorizationHandlers;
using MathEvent.Converters.Organizations.DTOs;
using MathEvent.Converters.Organizations.Models;
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
    public class OrganizationsController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly OrganizationService _organizationService;

        private readonly UserService _userService;

        private readonly IAuthorizationService _authorizationService;

        public OrganizationsController(
            IMapper mapper,
            OrganizationService organizationService,
            UserService userService,
            IAuthorizationService authorizationService)
        {
            _mapper = mapper;
            _organizationService = organizationService;
            _userService = userService;
            _authorizationService = authorizationService;
        }

        // GET api/Organizations/?key1=value1&key2=value2
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<OrganizationReadModel>>> ListAsync([FromQuery] IDictionary<string, string> filters)
        {
            var organizationResult = await _organizationService.ListAsync(filters);

            if (organizationResult.Succeeded && organizationResult.Entity is not null)
            {
                return Ok(organizationResult.Entity);
            }

            return NotFound(organizationResult.Messages);
        }

        // GET api/Organizations/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<OrganizationReadModel>> RetrieveAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest($"id = {id} less then 0");
            }

            var organizationResult = await _organizationService.RetrieveAsync(id);

            if (organizationResult.Succeeded && organizationResult.Entity is not null)
            {
                return Ok(organizationResult.Entity);
            }

            return NotFound(organizationResult.Messages);
        }

        // POST api/Organizations
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] OrganizationCreateModel organizaionCreateModel)
        {
            var userResult = await _userService.GetCurrentUserAsync(User);

            if (!userResult.Succeeded || userResult.Entity is null)
            {
                return StatusCode(401);
            }

            organizaionCreateModel.ManagerId = userResult.Entity.Id;
            var createResult = await _organizationService.CreateAsync(organizaionCreateModel);

            if (createResult.Succeeded)
            {
                var createdEvent = createResult.Entity;

                if (createdEvent is null)
                {
                    return Ok();
                }

                return StatusCode(201, createdEvent.Id);
            }
            else
            {
                return StatusCode(500, createResult.Messages);
            }
        }

        // PUT api/Organizations/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] OrganizationUpdateModel organizationUpdateModel)
        {
            if (id < 0)
            {
                return BadRequest($"id = {id} less then 0");
            }

            if (organizationUpdateModel.ManagerId is null)
            {
                return BadRequest("Manager id is null");
            }

            var organizationResult = await _organizationService.GetOrganizationEntityAsync(id);

            if (!organizationResult.Succeeded)
            {
                return NotFound(organizationResult.Messages);
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, organizationResult.Entity, Operations.Update);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(403);
            }

            var updateResult = await _organizationService.UpdateAsync(id, organizationUpdateModel);

            if (updateResult.Succeeded)
            {
                var updatedOrganization = updateResult.Entity;

                if (updatedOrganization is null)
                {
                    return Ok(id);
                }

                return Ok(updatedOrganization);
            }
            else
            {
                return StatusCode(500, updateResult.Messages);
            }
        }

        // PATCH api/Organizations/{id}
        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialUpdateAsync(int id, [FromBody] JsonPatchDocument<OrganizationUpdateModel> patchDocument)
        {
            if (id < 0)
            {
                return BadRequest($"id = {id} less then 0");
            }

            if (patchDocument is null)
            {
                return BadRequest("Patch document is null");
            }

            var organizationResult = await _organizationService.GetOrganizationEntityAsync(id);

            if (!organizationResult.Succeeded || organizationResult.Entity is null)
            {
                return NotFound(organizationResult.Messages);
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, organizationResult.Entity, Operations.Update);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(403);
            }

            var organizationEntity = organizationResult.Entity;
            var organizationDTO = _mapper.Map<OrganizationDTO>(organizationEntity);
            var organizationToPatch = _mapper.Map<OrganizationUpdateModel>(organizationDTO);
            patchDocument.ApplyTo(organizationToPatch, ModelState);

            if (!TryValidateModel(organizationToPatch))
            {
                return ValidationProblem(ModelState);
            }

            if (organizationToPatch.ManagerId is null)
            {
                return BadRequest("Manager id is null");
            }

            var updateResult = await _organizationService.UpdateAsync(id, organizationToPatch);

            if (updateResult.Succeeded)
            {
                var updatedOrganization = updateResult.Entity;

                if (updatedOrganization is null)
                {
                    return Ok(id);
                }

                return Ok(updatedOrganization);
            }
            else
            {
                return StatusCode(500, updateResult.Messages);
            }
        }

        // DELETE api/Organizations/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DestroyAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest($"id = {id} less then 0");
            }

            var organizationResult = await _organizationService.GetOrganizationEntityAsync(id);

            if (!organizationResult.Succeeded)
            {
                return NotFound(organizationResult.Messages);
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, organizationResult.Entity, Operations.Delete);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(403);
            }

            var deleteResult = await _organizationService.DeleteAsync(id);

            if (deleteResult.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(500, deleteResult.Messages);
            }
        }

        // GET api/Organizations/Statistics/?key1=value1&key2=value2
        [HttpGet("Statistics/")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SimpleStatistics>>> StatisticsAsync([FromQuery] IDictionary<string, string> filters)
        {
            var organizationResult = await _organizationService.GetSimpleStatistics(filters);

            if (organizationResult.Succeeded && organizationResult.Entity is not null)
            {
                return Ok(organizationResult.Entity);
            }

            return StatusCode(500, organizationResult.Messages);
        }
    }
}

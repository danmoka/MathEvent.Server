using AutoMapper;
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

        public OrganizationsController(IMapper mapper, OrganizationService organizationService)
        {
            _mapper = mapper;
            _organizationService = organizationService;
        }

        // GET api/Organizations/?key1=value1&key2=value2
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<OrganizationReadModel>>> ListAsync([FromQuery] IDictionary<string, string> filters)
        {
            var organizationResult = await _organizationService.ListAsync(filters);

            if (organizationResult.Succeeded)
            {
                var organizationReadModels = organizationResult.Entity;

                if (organizationReadModels is not null)
                {
                    return Ok(organizationReadModels);
                }
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
                return BadRequest("id < 0");
            }

            var organizationResult = await _organizationService.RetrieveAsync(id);

            if (organizationResult.Succeeded)
            {
                var organizationReadModel = organizationResult.Entity;

                if (organizationReadModel is not null)
                {
                    return Ok(organizationReadModel);
                }
            }

            return NotFound(organizationResult.Messages);
        }

        // POST api/Organizations
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] OrganizaionCreateModel organizaionCreateModel)
        {
            if (!TryValidateModel(organizaionCreateModel))
            {
                return ValidationProblem(ModelState);
            }

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
                return BadRequest(createResult.Messages);
            }
        }

        // PUT api/Organizations/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] OrganizationUpdateModel organizationUpdateModel)
        {
            if (id < 0)
            {
                return BadRequest("id < 0");
            }

            var organizationResult = await _organizationService.GetOrganizationEntityAsync(id);

            if (!organizationResult.Succeeded)
            {
                return NotFound(organizationResult.Messages);
            }

            if (!TryValidateModel(organizationUpdateModel))
            {
                return ValidationProblem(ModelState);
            }

            var updateResult = await _organizationService.UpdateAsync(id, organizationUpdateModel);

            if (updateResult.Succeeded)
            {
                return Ok(id);
            }
            else
            {
                return BadRequest(updateResult.Messages);
            }
        }

        // PATCH api/Organizations/{id}
        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialUpdateAsync(int id, [FromBody] JsonPatchDocument<OrganizationUpdateModel> patchDocument)
        {
            if (id < 0)
            {
                return BadRequest("id < 0");
            }

            if (patchDocument is null)
            {
                return BadRequest("Patch document is null");
            }

            var organizationResult = await _organizationService.GetOrganizationEntityAsync(id);

            if (!organizationResult.Succeeded)
            {
                return NotFound(organizationResult.Messages);
            }

            var organizationEntity = organizationResult.Entity;

            if (organizationEntity is null)
            {
                return NotFound();
            }

            var organizationDTO = _mapper.Map<OrganizationDTO>(organizationEntity);
            var organizationToPatch = _mapper.Map<OrganizationUpdateModel>(organizationDTO);
            patchDocument.ApplyTo(organizationToPatch, ModelState);

            if (!TryValidateModel(organizationToPatch))
            {
                return ValidationProblem(ModelState);
            }

            var updateResult = await _organizationService.UpdateAsync(id, organizationToPatch);

            if (updateResult.Succeeded)
            {
                return Ok(id);
            }
            else
            {
                return BadRequest(updateResult.Messages);
            }
        }

        // DELETE api/Organizations/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DestroyAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest("id < 0");
            }

            var organizationResult = await _organizationService.GetOrganizationEntityAsync(id);

            if (!organizationResult.Succeeded)
            {
                return NotFound(organizationResult.Messages);
            }

            var deleteResult = await _organizationService.DeleteAsync(id);

            if (deleteResult.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(deleteResult.Messages);
            }
        }

        // GET api/Organizations/Statistics/?key1=value1&key2=value2
        [HttpGet("Statistics/")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<SimpleStatistics>>> StatisticsAsync([FromQuery] IDictionary<string, string> filters)
        {
            var organizationResult = await _organizationService.GetSimpleStatistics(filters);

            if (organizationResult.Succeeded)
            {
                return Ok(organizationResult.Entity);
            }

            return NotFound(organizationResult.Messages);
        }
    }
}

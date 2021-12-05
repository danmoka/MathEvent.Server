using AutoMapper;
using MathEvent.AuthorizationHandlers;
using MathEvent.Contracts.Services;
using MathEvent.Contracts.Validators;
using MathEvent.DTOs.Organizations;
using MathEvent.Models.Organizations;
using MathEvent.Models.Others;
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
    public class OrganizationsController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly IOrganizationService _organizationService;

        private readonly IUserService _userService;

        private readonly IAuthorizationService _authorizationService;

        private readonly IOrganizationCreateModelValidator _organizationCreateModelValidator;

        private readonly IOrganizationUpdateModelValidator _organizationUpdateModelValidator;

        public OrganizationsController(
            IMapper mapper,
            IOrganizationService organizationService,
            IUserService userService,
            IAuthorizationService authorizationService,
            IOrganizationCreateModelValidator organizationCreateModelValidator,
            IOrganizationUpdateModelValidator organizationUpdateModelValidator)
        {
            _mapper = mapper;
            _organizationService = organizationService;
            _userService = userService;
            _authorizationService = authorizationService;
            _organizationCreateModelValidator = organizationCreateModelValidator;
            _organizationUpdateModelValidator = organizationUpdateModelValidator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<OrganizationReadModel>>> List([FromQuery] IDictionary<string, string> filters)
        {
            var organizations = await _organizationService.List(filters);

            return Ok(organizations);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<OrganizationReadModel>> Retrieve([FromRoute] int id)
        {
            if (id < 0)
            {
                return BadRequest($"id={id} меньше 0");
            }

            var organization = await _organizationService.Retrieve(id);

            if (organization is null)
            {
                return NotFound($"Организация с id={id} не найдена");
            }

            return Ok(organization);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] OrganizationCreateModel organizaionCreateModel)
        {
            var validationResult = await _organizationCreateModelValidator.Validate(organizaionCreateModel);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var organization = _mapper.Map<OrganizationReadModel>(_mapper.Map<OrganizationDTO>(organizaionCreateModel));

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, organization, Operations.Create);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя создать организацию");
            }

            var createdOrganization = await _organizationService.Create(organizaionCreateModel);

            if (createdOrganization is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время создания организации");
            }

            return CreatedAtAction(nameof(Retrieve), new { id = createdOrganization.Id }, createdOrganization);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] OrganizationUpdateModel organizationUpdateModel)
        {
            if (id < 0)
            {
                return BadRequest($"id={id} меньше 0");
            }

            var validationResult = await _organizationUpdateModelValidator.Validate(organizationUpdateModel);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var organization = await _organizationService.Retrieve(id);

            if (organization is null)
            {
                return NotFound($"Организация с id={id} не найдена");
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, organization, Operations.Update);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя редактировать организацию с id={id}");
            }

            var updatedOrganization = await _organizationService.Update(id, organizationUpdateModel);

            if (updatedOrganization is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время обновления события");
            }

            return Ok(updatedOrganization);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> PartialUpdateAsync([FromRoute] int id, [FromBody] JsonPatchDocument<OrganizationUpdateModel> patchDocument)
        {
            if (id < 0)
            {
                return BadRequest($"id={id} меньше 0");
            }

            if (patchDocument is null)
            {
                return BadRequest("Тело запроса не задано");
            }

            var organization = await _organizationService.Retrieve(id);

            if (organization is null)
            {
                return NotFound($"Организация с id={id} не найдена");
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, organization, Operations.Update);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя редактировать организацию с id={id}");
            }

            var organizationDTO = _mapper.Map<OrganizationDTO>(organization);
            var organizationToPatch = _mapper.Map<OrganizationUpdateModel>(organizationDTO);
            patchDocument.ApplyTo(organizationToPatch, ModelState);

            var validationResult = await _organizationUpdateModelValidator.Validate(organizationToPatch);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var updatedOrganization = await _organizationService.Update(id, organizationToPatch);

            if (updatedOrganization is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка во время обновления события");
            }

            return Ok(updatedOrganization);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Destroy([FromRoute] int id)
        {
            if (id < 0)
            {
                return BadRequest($"id={id} меньше 0");
            }

            var organization = await _organizationService.Retrieve(id);

            if (organization is null)
            {
                return NotFound($"Организация с id={id} не найдена");
            }

            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, organization, Operations.Delete);

            if (!authorizationResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status403Forbidden, $"Вам нельзя удалять организацию с id={id}");
            }

            await _organizationService.Delete(id);

            return NoContent();
        }

        [HttpGet("Statistics/")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ChartData>>> Statistics([FromQuery] IDictionary<string, string> filters)
        {
            var statistics = await _organizationService.GetOrganizationsStatistics(filters);

            return Ok(statistics);
        }
    }
}

using MathEvent.Converters.Files.Models;
using MathEvent.Converters.Others;
using MathEvent.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MathEvent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IUserService _userService;

        public FilesController(IFileService fileService, IUserService userService)
        {
            _fileService = fileService;
            _userService = userService;
        }

        // GET api/Files/?key1=value1&key2=value2
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileReadModel>>> ListAsync([FromQuery] IDictionary<string, string> filters)
        {
            var fileReadModels = await _fileService.ListAsync(filters);

            if (fileReadModels is not null)
            {
                return Ok(fileReadModels);
            }

            return NotFound();
        }

        // GET api/Files/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<FileReadModel>> RetrieveAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            var fileReadModel = await _fileService.RetrieveAsync(id);

            if (fileReadModel is not null)
            {
                return Ok(fileReadModel);
            }

            return NotFound();
        }

        // POST api/Files
        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] FileCreateModel fileCreateModel)
        {
            if (!TryValidateModel(fileCreateModel))
            {
                return ValidationProblem(ModelState);
            }

            var user = await _userService.GetCurrentUserAsync(User);
            fileCreateModel.AuthorId = user.Id;
            var createResult = await _fileService.CreateAsync(fileCreateModel);

            if (createResult.Succeeded)
            {
                var createdFile = createResult.Entity;

                if (createdFile is null)
                {
                    return StatusCode(201);
                }

                return StatusCode(201, createdFile);
            }
            else
            {
                return BadRequest(createResult.Messages);
            }
        }

        // PUT api/Files/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] FileUpdateModel fileUpdateModel)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            if (await _fileService.GetFileEntityAsync(id) is null)
            {
                return NotFound();
            }

            if (!TryValidateModel(fileUpdateModel))
            {
                return ValidationProblem(ModelState);
            }

            var updateResult = await _fileService.UpdateAsync(id, fileUpdateModel);

            if (updateResult.Succeeded)
            {
                var createdFile = updateResult.Entity;

                if (createdFile is null)
                {
                    return Ok(id);
                }

                return Ok(createdFile);
            }
            else
            {
                return BadRequest(updateResult.Messages);
            }
        }

        // DELETE api/Files/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DestroyAsync(int id)
        {
            if (id < 0)
            {
                return BadRequest();
            }

            if (await _fileService.GetFileEntityAsync(id) is null)
            {
                return NotFound();
            }

            var deleteResult = await _fileService.DeleteAsync(id);

            if (deleteResult.Succeeded)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(deleteResult.Messages);
            }
        }

        // POST api/Files/Upload
        [HttpPost("Upload")]
        public async Task<ActionResult> Upload([FromForm] IEnumerable<IFormFile> files, [FromQuery] string parentId, [FromQuery] string ownerId)
        {
            int? parent = null;
            int? owner = null;

            if (int.TryParse(parentId, out int parentParam))
            {
                parent = parentParam;
            }

            if (int.TryParse(ownerId, out int ownerParam))
            {
                owner = ownerParam;
            }


            foreach (var formFile in files)
            {
                var checkResult = _fileService.IsCorrectFile(formFile);

                if (!checkResult.Succeeded)
                {
                    return BadRequest(checkResult.Messages);
                }
            }

            var user = await _userService.GetCurrentUserAsync(User);
            var fileIds = new List<int>();

            foreach (var formFile in files)
            {
                var fileCreateModel = new FileCreateModel
                {
                    Name = Path.GetFileNameWithoutExtension(formFile.FileName),
                    Hierarchy = null,
                    ParentId = parent,
                    AuthorId = user.Id,
                    OwnerId = owner
                };

                var createResult = await _fileService.Upload(formFile, fileCreateModel);

                if (!createResult.Succeeded)
                {
                    return BadRequest(createResult.Messages);
                }
                else
                {
                    fileIds.Add(createResult.Entity.Id);
                }
            }

            return Ok(fileIds);
        }

        // GET api/Files/Download
        [HttpGet("Download/{id}")]
        public async Task<ActionResult> Download(int id)
        {
            var file = await _fileService.GetFileEntityAsync(id);

            if (file is null)
            {
                return NotFound();
            }

            if (file.Hierarchy is not null)
            {
                return BadRequest();
            }

            var fileStream = await _fileService.Download(id);

            if (fileStream is null)
            {
                return BadRequest();
            }

            return File(fileStream, "application/octet-stream", $"\"{file.Name}{file.Extension}\"");
        }

        // GET api/Files/Breadcrumbs/{id}
        [HttpGet("Breadcrumbs/{id}")]
        public async Task<ActionResult<IEnumerable<Breadcrumb>>> GetBreadcrumbs(int id)
        {
            var result = await _fileService.GetBreadcrumbs(id);

            if (result.Succeeded)
            {
                return Ok(result.Entity);
            }
            else
            {
                return BadRequest(result.Messages);
            }
        }
    }
}

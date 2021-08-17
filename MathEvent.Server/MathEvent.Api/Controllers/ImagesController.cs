using MathEvent.Services.Services.DataPath;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MathEvent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly DataPathService _dataPathService;

        public ImagesController(DataPathService dataPathService)
        {
            _dataPathService = dataPathService;
        }

        // GET api/Images/?src=value
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetImage([FromQuery] string src)
        {
            // TODO: безопасный ли этот метод?
            var image = _dataPathService.GetFileStream(src);

            return Ok(image);
        }
    }
}

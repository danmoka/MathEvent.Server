using MathEvent.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MathEvent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebRootDataController : ControllerBase
    {
        private readonly IDataPathWorker _dataPathWorker;

        public WebRootDataController(IDataPathWorker dataPathService)
        {
            _dataPathWorker = dataPathService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetWebRootFile([FromQuery] string src)
        {
            var file = _dataPathWorker.GetWebRootFileStream(src);

            return Ok(file);
        }
    }
}

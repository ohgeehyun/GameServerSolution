using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("ping")]
        public string Ping() => "pong";

        [HttpGet("alive")]

        public string Alive() => "api-server alive.";
    }
}

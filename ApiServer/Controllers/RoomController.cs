using ApiServer.Service.Redis.Room;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;

namespace ApiServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly RedisRoomService _redisRoomService;

        public RoomController(RedisRoomService redisRoomService)
        {
            _redisRoomService = redisRoomService;
        }

        //Get
        [HttpGet("list")]
        public async Task<IActionResult> GetRoomList()
        {
            var roomKeys = await _redisRoomService.GetRoomListAsync();
            return Ok(roomKeys);
        }

    }
}

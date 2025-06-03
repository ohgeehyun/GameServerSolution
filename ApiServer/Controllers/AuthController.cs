using ApiServer.DB;
using ApiServer.DB.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        public class LoginRequest
        {
            public string UserId { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class LoginResponse
        {
            public bool Status { get; set; }
            public string Message { get; set; } = string.Empty;
            public string Token { get; set; } = string.Empty;
        }

        [HttpPost("login/user")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            //유효성 검사
            if(string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new LoginResponse
                {
                    Status = false,
                    Message = "UserId 또는 Password가 없습니다."
                });
            }

            var (success, message, user) = await _authService.ValidateUserAsync(request.UserId,request.Password);

            if (!success)
                return Unauthorized(new LoginResponse { Status = false , Message = message});

            // 성공하면 jwt 토큰 생성해서 리턴 등 
            return Ok(new LoginResponse { Status = true, Message = message , Token = "Jwt 토큰 값" });
        }

        [HttpPost("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

    }
}

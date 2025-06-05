using ApiServer.DB;
using ApiServer.Service.Auth;
using ApiServer.Service.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ApiServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AuthService _authService;
        private readonly JwtService _jwtService;

        public AuthController(AuthService authService, JwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        public class LoginRequest
        {
            public string UserId { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class LoginResponse
        {
            public bool Status { get; set; } = false;
            public string Message { get; set; } = string.Empty;
            public string Token { get; set; } = string.Empty;
        }

        [EnableRateLimiting("LoginPolicy")]
        [HttpPost("login/user")]
        [AllowAnonymous] //jwt 인증 제외 
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            //유효성 검사
            if(string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.Password))
            {
                //HTTP Status Codes 400
                return BadRequest(new LoginResponse
                {
                    Status = false,
                    Message = "UserId 또는 Password가 없습니다."
                });
            }

            var (success, message, user) = await _authService.ValidateUserAsync(request.UserId,request.Password);

            //HTTP Status Codes 401
            if (!success)
                return Unauthorized(new LoginResponse { Status = false , Message = message});

            string token = _jwtService.GenerateToken(request.UserId);
       
            // 성공하면 jwt 토큰 생성해서 리턴 등 
            return Ok(new LoginResponse { Status = true, Message = message , Token = token });
        }





        [EnableRateLimiting("ApiPolicy")]
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

    }
}

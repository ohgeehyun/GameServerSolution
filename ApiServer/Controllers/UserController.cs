using ApiServer.DB;
using ApiServer.Service.User;
using ApiServer.Service.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiServer.DB.Model;

[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly JwtService _jwtService;

    private readonly GameDbContext _context;

    public UserController(UserService userService, JwtService jwtService, GameDbContext context)
    {
        _userService = userService;
        _jwtService = jwtService;
        _context = context;
    }



    public class RegistInfoRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UserNickName { get; set; } = string.Empty;
    }

    public class RegistInfoResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
    }


    [HttpPost("user/regist")]
    public async Task<ActionResult<RegistInfoResponse>> userRegister([FromBody] RegistInfoRequest request)
    {
        //유효성 검사
        if (string.IsNullOrEmpty(request.UserId) ||string.IsNullOrEmpty(request.Password) ||string.IsNullOrEmpty(request.UserNickName))
        {
            //HTTP Status Codes 400
            return BadRequest(new RegistInfoResponse
            {
                Status = false,
                Message = "아이디 또는 비밀번호를 입력해 주세요."
            }); 
        }

        var existingUser = await _userService.FindUser(request.UserId);

        // //HTTP Status Codes 409
        if (!existingUser)
            return Conflict(new RegistInfoResponse { Status = false, Message = "이미 존재하는 사용자 입니다." });

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var newUser = new Users
        {
            UserId = request.UserId,
            Password = hashedPassword,
            Nickname = request.UserNickName
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok(new RegistInfoResponse{Status = true,Message = "회원가입 성공"});

    }

}


using ApiServer.DB.Model;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.DB.Service
{
    //인증관련 된 기능을 담당하는 서비스 클래스
    public class AuthService
    {
        private readonly GameDbContext _context;

        public AuthService(GameDbContext context)
        {
            _context = context;
        }

        public async Task<(bool success, string message, Users? user)> ValidateUserAsync(string userId,string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return (false, "아이디가 존재하지 않습니다.", null);

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password,user.Password);

            if (!isPasswordValid)
                return (false, "비밀번호가 틀렸습니다.", null);

            return (true, "로그인 성공", user);
        }

        
    }
}

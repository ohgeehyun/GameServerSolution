using ApiServer.DB;
using ApiServer.DB.Model;
using global::ApiServer.DB.Model;
using global::ApiServer.DB;
using Microsoft.EntityFrameworkCore;

namespace ApiServer.Service.User
{
    //유저 로그인 인증 관련 된 기능을 담당하는 서비스 클래스
    public class UserService
    {
        private readonly GameDbContext _context;

        public UserService(GameDbContext context)
        {
            _context = context;
        }

        //bool 반환 - 중복 유저 체크
        internal async Task<bool> FindUser(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return true;

            return false;
        }


    }
}

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using RoomServer.UserData;

namespace RoomServer.Manager.Auth
{
    public class JwtUtils
    {
        public static readonly Dictionary<string, Action<User, string>> UserClaimMappings = new()
        {
            { JwtRegisteredClaimNames.Sub,(u,val) => u.userId = val},
            { "nickname",(u,val)=>u.userNick = val},
            { JwtRegisteredClaimNames.Jti,(u,val) => u.jwtId = val}
        };

        public static void MapClaimsToUser(User user, ClaimsPrincipal principal)
        {
            foreach (var (key, action) in UserClaimMappings)
            {
                var value = principal.FindFirst(key)?.Value;
                if (!string.IsNullOrEmpty(value))
                     action(user, value);
            }

            //출력용 코드 필요시 없어지면 삭제 
            var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var nickname = principal.FindFirst("nickname")?.Value;
            var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            Console.WriteLine($"JWT 인증 성공. 사용자 ID: {userId} 사용자 닉네임:{nickname} 토큰 식별번호:{jti}");
        }
    }

}

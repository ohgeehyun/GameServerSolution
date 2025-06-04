using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiServer.Service.Jwt;


namespace ApiServer.Service.Jwt
{
    public class JwtService 
    {
        private readonly string _secretKey; //서명부분을 만들기 위한 시크릿 키
        private readonly string _issuer; // 토큰 발행자
        private readonly string _audience;//토큰 대상자

        public JwtService(string secretKey,string issuer,string audience)
        {
            _secretKey = secretKey;
            _issuer = issuer;   
            _audience = audience;   
        }

        public string GenerateToken(string userId, int expireMinutes = 120)
        {
            //대칭키를 바이트 배열로 변환해서 보안키 생성
            var sercurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

            //대칭키를 사용해 SHA 256 서명을 하기위한 증명 생성
            var credentials = new SigningCredentials(sercurityKey, SecurityAlgorithms.HmacSha256);

            //클레임 정의
            var claims = new[]
            {

                //표준 클레임
                //iss 발행자
                //sub 주체
                //aud 대상자
                //exp 만료시간
                //nbf 활성화 시간 
                //iat 발급시간
                //jti jwt식별자

                //jwt 표준 클레임
                new Claim(JwtRegisteredClaimNames.Sub,userId),
                //jwt id - 토큰 고유값 재사용 방지용 랜덤 guid
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                    issuer: _issuer,
                    audience: _audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                    signingCredentials: credentials
                );

            //토큰을 문자열로 직렬화해서 반환
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

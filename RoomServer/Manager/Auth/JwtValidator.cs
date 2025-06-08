using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace RoomServer.Manager.Auth
{

    public class JwtValidator
    {
        private static readonly object _lock = new();
        #region singloton
        private static JwtValidator? _instance;

        public static JwtValidator Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new JwtValidator();
                        }
                    }
                }
                return _instance;
            }
        }
        #endregion;

        public enum JwtValidationResult
        {
            Success,
            Expired,
            InvalidSignature,
            InvalidAudience,
            InvalidIssuer,
            OtherError
        }

        private readonly string _secretKey = string.Empty;
        private readonly string _validIssuer = string.Empty;
        private readonly string _validAudience = string.Empty;

        private JwtValidator()
        {
            //초기화
            _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                   ?? throw new Exception("JWT_SECRET_KEY not set");

            _validIssuer = "OG_server";
            _validAudience = "OG_client";

        }

        public ClaimsPrincipal? Validatetoken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _validIssuer,
                ValidateAudience = true,
                ValidAudience = _validAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1) // 시계 오차 허용 범위
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine($"JWT 검증 실패: {ex.Message}");
                return null;
            }
        }

    }
}

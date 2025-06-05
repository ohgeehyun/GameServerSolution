using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ApiServer.Service.Jwt;
using System.Text;
using ApiServer.DB;
using Microsoft.EntityFrameworkCore;
using ApiServer.Extensions;
using ApiServer.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("http://0.0.0.0:5251"); // docker 내부에서 돌아갈떄 5251포트를 사용

/*------------------------------------------ 
                   jwt
 -----------------------------------------*/
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

if (string.IsNullOrEmpty(jwtKey))
    throw new Exception("jwt 시크릿 키가 설졍되지 않았습니다.");

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "OG_server",
            ValidAudience = "OG_Client",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

/*------------------------------------------ 
                  Services
 -----------------------------------------*/
builder.Services.RegisterAppServices(jwtKey);


/*------------------------------------------ 
                  mysql
 -----------------------------------------*/
string? connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new Exception("MYSQL 환경변수가 설정되지 않았습니다.");

builder.Services.AddDbContext<GameDbContext>(options => options.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString)));




/*--------------------------------------- 
                policy
 -----------------------------------------*/


/*--------------------------------------- 
                 build
 -----------------------------------------*/
var app = builder.Build();

app.UseRateLimiter(); //미들웨어로 로그인 횟수 제한 등록

app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthorization();

app.MapControllers();
app.Run();

using ApiServer.Service;
using ApiServer.Service.Auth;
using ApiServer.Service.User;
using ApiServer.Service.Jwt;
using ApiServer.Service.Redis;
using StackExchange.Redis;
using ApiServer.Service.Redis.Room;


namespace ApiServer.Extensions
{
    public static class ServiceExtensions
    {
        public static void RegisterAppServices(this IServiceCollection services,string jwtKey,string redisHost)
        {
            //scoped
            services.AddScoped<AuthService>();
            services.AddScoped<UserService>();
            services.AddScoped<RedisRoomService>();

            //singleton
            services.AddSingleton(new JwtService(jwtKey, "OG_server", "OG_client"));
            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisHost));

            //Transient



            // Rate Limiting 정책 등록
            services.AddCustomRateLimiters();
        }
    }
}
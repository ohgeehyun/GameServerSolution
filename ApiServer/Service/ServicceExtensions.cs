using ApiServer.Service;
using ApiServer.Service.Auth;
using ApiServer.Service.User;
using ApiServer.Service.Jwt;


namespace ApiServer.Extensions
{
    public static class ServiceExtensions
    {
        public static void RegisterAppServices(this IServiceCollection services,string jwtKey)
        {
            //scoped
            services.AddScoped<AuthService>();
            services.AddScoped<UserService>();

            //singleton
            services.AddSingleton(new JwtService(jwtKey, "OG_server", "OG_client"));


            //Transient
        }
    }
}
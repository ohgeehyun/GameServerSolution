using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Threading.RateLimiting;

using ApiServer.Extensions;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddCustomRateLimiters(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {

            //로그인
            options.AddPolicy("LoginPolicy", context =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown Ip";
                return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromSeconds(10),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });

            //회원 가입
            options.AddPolicy("RegisterPolicy", context =>
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 2,
                    Window = TimeSpan.FromMinutes(10),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });

            //일반 api
            options.AddPolicy("ApiPolicy", context =>
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromSeconds(10),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
            });
        });
        return services;
    }







}

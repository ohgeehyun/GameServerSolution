using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using ApiServer.DB.Mysql;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<GameDbContext>
{
    public GameDbContext CreateDbContext(string[] args)
    {
        // 환경 변수 또는 appsettings.json에서 설정 읽기
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables() // 환경변수 우선
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var connectionString = configuration["MYSQL_CONNECTION_STRING"];
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception("MYSQL_CONNECTION_STRING 환경변수를 찾을 수 없습니다.");

        var optionsBuilder = new DbContextOptionsBuilder<GameDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new GameDbContext(optionsBuilder.Options);
    }
}
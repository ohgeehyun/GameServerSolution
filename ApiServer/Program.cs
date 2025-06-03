

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("http://0.0.0.0:5251"); // docker 내부에서 돌아갈떄 5251포트를 사용

string? connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");

if(string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("MYSQL 환경변수가 설정되지 않았습니다.");
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthorization();

//app.MapControllers();

app.Run();

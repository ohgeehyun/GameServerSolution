# 빌드 단계
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln ./
COPY NetworkCore/ ./NetworkCore/
COPY TcpServer/ ./TcpServers/

WORKDIR /app/TcpServers
RUN dotnet publish -c Release -o /out


# 실행 단계
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /out .

EXPOSE 7777
ENTRYPOINT ["dotnet", "TcpServer.dll"]
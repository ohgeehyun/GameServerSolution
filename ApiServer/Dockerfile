# 빌드 단계
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln ./
COPY NetworkCore/ ./NetworkCore/
COPY ApiServer/ ./ApiServer/

WORKDIR /app/ApiServer
RUN dotnet publish -c Release -o /out

# 실행 단계
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

ENV ASPNETCORE_HTTP_PORTS=5251 

# 컨테이너가 5251 포트에서 리스닝
EXPOSE 5251 
ENTRYPOINT ["dotnet", "ApiServer.dll"]
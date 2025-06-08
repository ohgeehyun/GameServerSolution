# 빌드 단계
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln ./
COPY NetworkCore/ ./NetworkCore/
COPY RoomServer/ ./RoomServer/

WORKDIR /app/RoomServer
RUN dotnet publish -c Release -o /out


# 실행 단계
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /out .

EXPOSE 7778
ENTRYPOINT ["dotnet", "RoomServer.dll"]
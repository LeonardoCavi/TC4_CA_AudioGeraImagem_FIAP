#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AudioGeraImagemAPI.API/AudioGeraImagemAPI.API.csproj", "AudioGeraImagemAPI.API/"]
COPY ["AudioGeraImagemAPI.Infra/AudioGeraImagemAPI.Infra.csproj", "AudioGeraImagemAPI.Infra/"]
COPY ["AudioGeraImagemAPI.Domain/AudioGeraImagemAPI.Domain.csproj", "AudioGeraImagemAPI.Domain/"]
COPY ["AudioGeraImagemAPI.UseCases/AudioGeraImagemAPI.UseCases.csproj", "AudioGeraImagemAPI.UseCases/"]
RUN dotnet restore "AudioGeraImagemAPI.API/AudioGeraImagemAPI.API.csproj"
COPY . .
WORKDIR "/src/AudioGeraImagemAPI.API"
RUN dotnet build "AudioGeraImagemAPI.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AudioGeraImagemAPI.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AudioGeraImagemAPI.API.dll"]
#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AudioGeraImagemWorker.Worker/AudioGeraImagemWorker.Worker.csproj", "AudioGeraImagemWorker.Worker/"]
COPY ["AudioGeraImagemWorker.Domain/AudioGeraImagemWorker.Domain.csproj", "AudioGeraImagemWorker.Domain/"]
COPY ["AudioGeraImagemWorker.Infra/AudioGeraImagemWorker.Infra.csproj", "AudioGeraImagemWorker.Infra/"]
COPY ["AudioGeraImagemWorker.UseCases/AudioGeraImagemWorker.UseCases.csproj", "AudioGeraImagemWorker.UseCases/"]
RUN dotnet restore "AudioGeraImagemWorker.Worker/AudioGeraImagemWorker.Worker.csproj"
COPY . .
WORKDIR "/src/AudioGeraImagemWorker.Worker"
RUN dotnet build "AudioGeraImagemWorker.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AudioGeraImagemWorker.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AudioGeraImagemWorker.Worker.dll"]
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/RPSLSGame.Api/RPSLSGame.Api.csproj", "src/RPSLSGame.Api/"]
COPY ["src/RPSLSGame.Application/RPSLSGame.Application.csproj", "src/RPSLSGame.Application/"]
COPY ["src/RPSLSGame.Domain/RPSLSGame.Domain.csproj", "src/RPSLSGame.Domain/"]
COPY ["src/RPSLSGame.Infrastructure/RPSLSGame.Infrastructure.csproj", "src/RPSLSGame.Infrastructure/"]

RUN dotnet restore "src/RPSLSGame.Api/RPSLSGame.Api.csproj"

COPY . .
WORKDIR "/src/src/RPSLSGame.Api"
RUN dotnet build "RPSLSGame.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RPSLSGame.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RPSLSGame.Api.dll"]

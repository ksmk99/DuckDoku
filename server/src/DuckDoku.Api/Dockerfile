FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Build.props
COPY server/src/DuckDoku.Api/DuckDoku.Api.csproj server/src/DuckDoku.Api/
COPY shared/DuckDoku.Contracts/DuckDoku.Contracts.csproj shared/DuckDoku.Contracts/
COPY shared/DuckDoku.Puzzle/DuckDoku.Puzzle.csproj shared/DuckDoku.Puzzle/

RUN dotnet restore server/src/DuckDoku.Api/DuckDoku.Api.csproj

COPY server/src/DuckDoku.Api/ server/src/DuckDoku.Api/
COPY shared/DuckDoku.Contracts/ shared/DuckDoku.Contracts/
COPY shared/DuckDoku.Puzzle/ shared/DuckDoku.Puzzle/

RUN dotnet publish server/src/DuckDoku.Api/DuckDoku.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore 



FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .
COPY content/ /content/

ENTRYPOINT ["dotnet", "DuckDoku.Api.dll"]

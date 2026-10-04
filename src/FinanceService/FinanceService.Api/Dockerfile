FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/FinanceService/ src/FinanceService/
RUN dotnet publish src/FinanceService/FinanceService.Api/FinanceService.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT ["dotnet", "FinanceService.Api.dll"]

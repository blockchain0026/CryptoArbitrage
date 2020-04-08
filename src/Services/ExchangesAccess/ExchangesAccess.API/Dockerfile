FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 4402
EXPOSE 44328

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/Services/ExchangesAccess/ExchangesAccess.API/ExchangesAccess.API.csproj src/Services/ExchangesAccess/ExchangesAccess.API/
RUN dotnet restore src/Services/ExchangesAccess/ExchangesAccess.API/ExchangesAccess.API.csproj
COPY . .
WORKDIR /src/src/Services/ExchangesAccess/ExchangesAccess.API
RUN dotnet build ExchangesAccess.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ExchangesAccess.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ExchangesAccess.API.dll"]

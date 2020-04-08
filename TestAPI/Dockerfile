FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 60715
EXPOSE 44348

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY TestAPI/TestAPI.csproj TestAPI/
RUN dotnet restore TestAPI/TestAPI.csproj
COPY . .
WORKDIR /src/TestAPI
RUN dotnet build TestAPI.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish TestAPI.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TestAPI.dll"]

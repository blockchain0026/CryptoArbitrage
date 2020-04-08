FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 63529
EXPOSE 44351

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/Services/Execution/Execution.API/Execution.API.csproj src/Services/Execution/Execution.API/
RUN dotnet restore src/Services/Execution/Execution.API/Execution.API.csproj
COPY . .
WORKDIR /src/src/Services/Execution/Execution.API
RUN dotnet build Execution.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Execution.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Execution.API.dll"]

CMD dotnet ef database update
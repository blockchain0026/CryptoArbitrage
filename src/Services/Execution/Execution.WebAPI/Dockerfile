FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 59927
EXPOSE 44334

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/Services/Execution/Execution.WebAPI/Execution.WebAPI.csproj src/Services/Execution/Execution.WebAPI/
RUN dotnet restore src/Services/Execution/Execution.WebAPI/Execution.WebAPI.csproj
COPY . .
WORKDIR /src/src/Services/Execution/Execution.WebAPI
RUN dotnet build Execution.WebAPI.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Execution.WebAPI.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Execution.WebAPI.dll"]

CMD dotnet ef database update
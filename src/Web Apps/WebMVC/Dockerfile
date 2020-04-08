FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 3684
EXPOSE 44324

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/Web Apps/WebMVC/WebMVC.csproj src/Web Apps/WebMVC/
RUN dotnet restore src/Web Apps/WebMVC/WebMVC.csproj
COPY . .
WORKDIR /src/src/Web Apps/WebMVC
RUN dotnet build WebMVC.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish WebMVC.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebMVC.dll"]

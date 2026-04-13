FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Crm.Api/Crm.Api.csproj", "src/Crm.Api/"]
RUN dotnet restore "src/Crm.Api/Crm.Api.csproj"

COPY . .
WORKDIR "/src/src/Crm.Api"
RUN dotnet publish "Crm.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Crm.Api.dll"]

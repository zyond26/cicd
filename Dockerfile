FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Web_Restaurant.csproj", "."]
RUN dotnet restore "./Web_Restaurant.csproj"
COPY . .
RUN dotnet build "./Web_Restaurant.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Web_Restaurant.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN mkdir -p /app/keys && chmod 700 /app/keys
VOLUME /app/keys
COPY --from=publish /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "Web_Restaurant.dll"]
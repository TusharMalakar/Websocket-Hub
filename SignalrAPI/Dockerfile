# Follow README.md to create, run and kill images and containers
# Note: These stage 1 and 2 builds image when we run build cmd

# Stage 1
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /build
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# Stage 2
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS final
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
EXPOSE 443
EXPOSE 8080
ENTRYPOINT ["dotnet", "SignalrAPI.dll"]

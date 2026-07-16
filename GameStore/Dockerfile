# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the project file first to leverage Docker layer caching on restore
COPY GameStore.csproj ./
RUN dotnet restore

# Copy the rest of the source and publish
COPY . ./
RUN dotnet publish -c Release -o /app/release --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/release ./

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "GameStore.dll"]
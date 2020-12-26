#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
#FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Supermarket.API/Supermarket.API.csproj", "Supermarket.API/"]
RUN dotnet restore "Supermarket.API/Supermarket.API.csproj"
COPY . .
WORKDIR "/src/Supermarket.API"
RUN dotnet build "Supermarket.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Supermarket.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Supermarket.API.dll"]
#FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
#WORKDIR /app
#COPY --from=publish /src/publish .
# ENTRYPOINT ["dotnet", "Colors.API.dll"]
# heroku uses the following
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Supermarket.API.dll



# NuGet restore
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY *.sln .
COPY Supermarket.API/*.csproj Supermarket.API/
RUN dotnet restore
COPY . .

# testing
FROM build AS testing
WORKDIR /src/Supermarket.API
RUN dotnet build


# publish
FROM build AS publish
WORKDIR /src/Supermarket.API
RUN dotnet publish -c Release -o /src/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY --from=publish /src/publish .
# ENTRYPOINT ["dotnet", "Supermarket.API.dll"]
# heroku uses the following
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Colors.API.dll
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build-env
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore 
COPY . .
WORKDIR "/src/Supermarket.API"
#RUN dotnet build "Supermarket.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish  -c Release -o out

FROM base AS final
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Supermarket.API.dll"]
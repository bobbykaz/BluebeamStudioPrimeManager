#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["PrimeCollaborationManager/PrimeCollaborationManager.csproj", "PrimeCollaborationManager/"]
COPY ["Studio.Api.Client/Studio.Api.Client.csproj", "Studio.Api.Client/"]
COPY ["Studio.Api.Model/Studio.Api.Model.csproj", "Studio.Api.Model/"]
RUN dotnet restore "PrimeCollaborationManager/PrimeCollaborationManager.csproj"
COPY . .
WORKDIR "/src/PrimeCollaborationManager"
RUN dotnet build "PrimeCollaborationManager.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PrimeCollaborationManager.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PrimeCollaborationManager.dll"]
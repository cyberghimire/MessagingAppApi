FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["MessagingApp.API.csproj", "."]
RUN dotnet restore "MessagingApp.API.csproj"
COPY ./MessagingApp.API ./MessagingApp.API
WORKDIR "/src/"
RUN dotnet build "MessagingApp.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MessagingApp.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS http://*:$PORT
ENTRYPOINT ["dotnet", "MessagingApp.API.dll"]
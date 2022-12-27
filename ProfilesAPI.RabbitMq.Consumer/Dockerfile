FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ProfilesAPI.RabbitMq.Consumer/ProfilesAPI.RabbitMq.Consumer.csproj", "ProfilesAPI.RabbitMq.Consumer/"]
RUN dotnet restore "ProfilesAPI.RabbitMq.Consumer/ProfilesAPI.RabbitMq.Consumer.csproj"
COPY . .
WORKDIR "/src/ProfilesAPI.RabbitMq.Consumer"
RUN dotnet build "ProfilesAPI.RabbitMq.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProfilesAPI.RabbitMq.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProfilesAPI.RabbitMq.Consumer.dll"]

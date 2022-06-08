FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000



FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MiSmart.API/MiSmart.API.csproj", "MiSmart.API/"]
RUN dotnet restore "MiSmart.API\MiSmart.API.csproj"
COPY . .
WORKDIR "/src/MiSmart.API"
RUN dotnet build "MiSmart.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MiSmart.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /src/wait-for-it.sh /app
RUN chmod +x ./*.sh
ENTRYPOINT ["./wait-for-it.sh", "postgres_image:5432" ,"--","dotnet", "MiSmart.API.dll"]

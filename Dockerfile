# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["LifeAdminPlatform.sln", "."]
COPY ["GCommon/GCommon.csproj", "GCommon/"]
COPY ["LifeAdmin/LifeAdmin.Web.csproj", "LifeAdmin/"]
COPY ["LifeAdmin.Tests/LifeAdmin.Tests.csproj", "LifeAdmin.Tests/"]
COPY ["LifeAdminData/LifeAdminData.csproj", "LifeAdminData/"]
COPY ["LifeAdminModels/LifeAdminModels.csproj", "LifeAdminModels/"]
COPY ["LifeAdminServices/LifeAdminServices.csproj", "LifeAdminServices/"]
COPY ["ViewModels/ViewModels.csproj", "ViewModels/"]

RUN dotnet restore "LifeAdminPlatform.sln"

COPY . .

WORKDIR /src/LifeAdmin
RUN dotnet publish "LifeAdmin.Web.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "LifeAdmin.Web.dll"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
#EXPOSE 80
EXPOSE 8080
VOLUME ["/letsencrypt"]
VOLUME ["/etc/letsencrypt"]

# Restore
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["../MyBuyingList.sln", "./"]
COPY ["../MyBuyingList/*.csproj", "./MyBuyingList/"]
COPY ["../*.dcproj", "./"]
COPY ["../MyBuyingList.Domain/*.csproj", "./MyBuyingList.Domain/"]
COPY ["../MyBuyingList.Application/*.csproj", "./MyBuyingList.Application/"]
COPY ["../MyBuyingList.Infrastructure/*.csproj", "./MyBuyingList.Infrastructure/"]
RUN dotnet restore "./MyBuyingList/MyBuyingList.Web.csproj"

# Build
COPY . .
WORKDIR "/src/MyBuyingList"
RUN dotnet build "MyBuyingList.Web.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "MyBuyingList.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Set the final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyBuyingList.Web.dll"]

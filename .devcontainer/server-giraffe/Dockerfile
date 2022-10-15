# Dockerfile

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
COPY ["./.devcontainer/server-giraffe/mono-install.sh", "."]
RUN bash mono-install.sh

WORKDIR /app
ENV IS_DOCKER true
ENV ASPNETCORE_URLS=http://+:80  
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
COPY ["src/Server.Giraffe/Server.Giraffe.fsproj", "src/Server.Giraffe/"]
RUN dotnet restore src/Server.Giraffe/Server.Giraffe.fsproj
COPY . .
RUN dotnet build ./src/Server.Giraffe/Server.Giraffe.fsproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish ./src/Server.Giraffe/Server.Giraffe.fsproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["dotnet", "Server.Giraffe.dll"]

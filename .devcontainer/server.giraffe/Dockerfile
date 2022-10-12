# Dockerfile

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#CMD cat /etc/os-release
RUN apt update && apt upgrade
RUN apt install apt-transport-https dirmngr gnupg ca-certificates -y
RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
RUN "deb https://download.mono-project.com/repo/debian stable-buster main" | tee /etc/apt/sources.list.d/mono-official-stable.list
RUN apt update
RUN apt install mono-complete -y

WORKDIR /app
#EXPOSE 5000
#EXPOSE 5001
ARG BUILD_CONFIGURATION=Debug
ENV ASPNETCORE_ENVIRONMENT=Development
ENV DOTNET_USE_POLLING_FILE_WATCHER=true  
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
ENTRYPOINT ["dotnet", "Server.Giraffe.dll"]

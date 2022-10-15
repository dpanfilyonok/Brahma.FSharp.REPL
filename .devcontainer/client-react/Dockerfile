FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

# Install node
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash
RUN apt-get update && apt-get install -y nodejs

WORKDIR /app
COPY . .

RUN dotnet tool restore
RUN dotnet paket install
RUN npm install

CMD [ "npm", "run", "start" ]

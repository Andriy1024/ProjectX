FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS publish
WORKDIR /src
COPY . .
RUN dotnet publish src/Services/ProjectX.Realtime/ProjectX.Realtime.API/ProjectX.Realtime.API.csproj -c Release -o /app/ProjectX.Realtime
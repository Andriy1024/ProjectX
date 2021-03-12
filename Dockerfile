FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS publish
WORKDIR /src
COPY . .
RUN dotnet publish src/Services/ProjectX.Realtime/ProjectX.Realtime.API/ProjectX.Realtime.API.csproj -c Release -o /app/ProjectX.Realtime
RUN dotnet publish src/Services/ProjectX.Blog/ProjectX.Blog.API/ProjectX.Blog.API.csproj -c Release -o /app/ProjectX.Blog
RUN dotnet publish src/Services/ProjectX.Identity/ProjectX.Identity.API/ProjectX.Identity.API.csproj -c Release -o /app/ProjectX.Identity
RUN dotnet publish src/Services/ProjectX.Messenger/ProjectX.Messenger.API/ProjectX.Messenger.API.csproj -c Release -o /app/ProjectX.Messenger
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app

FROM base AS final
WORKDIR /app
COPY --from=projectx-build /app/ProjectX.Realtime .
ENTRYPOINT ["dotnet", "ProjectX.Realtime.API.dll"]
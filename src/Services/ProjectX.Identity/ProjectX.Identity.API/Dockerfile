FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM base AS final
WORKDIR /app
COPY --from=projectx-build /app/ProjectX.Identity .
ENTRYPOINT ["dotnet", "ProjectX.Identity.API.dll"]
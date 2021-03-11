FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app

FROM base AS final
WORKDIR /app
COPY --from=projectx-build /app/ProjectX.Blog .
ENTRYPOINT ["dotnet", "ProjectX.Blog.API.dll"]
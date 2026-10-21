# Image unique : publie Gprojet-Backend, qui embarque le front Blazor WASM (wwwroot/_framework).
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY .editorconfig global.json Directory.Build.props Directory.Packages.props ./
COPY src/ src/
RUN dotnet publish src/Gprojet-Backend/Gprojet-Backend.csproj -c Release -o /out

# Bundle de migrations EF (cible `migrations`) : GpDbContextFactory lit ConnectionStrings__Gp à l'exécution.
FROM build AS migrations-build
COPY .config/ .config/
# Chaîne factice, build uniquement (aucune connexion) ; la vraie est fournie à l'exécution.
RUN dotnet tool restore \
 && ConnectionStrings__Gp='Host=build;Database=build' dotnet ef migrations bundle --project src/Gprojet-Backend/Gprojet-Backend.csproj \
    --self-contained -r linux-musl-x64 --force -o /efbundle

FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-alpine AS migrations
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
WORKDIR /app
COPY --from=migrations-build /efbundle .
USER $APP_UID
ENTRYPOINT ["./efbundle"]

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
# ICU : le projet n'est pas en InvariantGlobalization (cultures fr-MA / ar-MA).
RUN apk add --no-cache icu-libs tzdata
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    DOTNET_gcServer=0
WORKDIR /app
COPY --from=build /out .
# Utilisateur non-root "app" fourni par l'image de base. Chaîne de connexion : ConnectionStrings__Gp à l'exécution.
USER $APP_UID
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=3s --start-period=20s --retries=3 \
  CMD wget -qO- http://localhost:8080/health || exit 1
# Pas d'ENTRYPOINT : `docker run image <cmd>` remplace le démarrage de l'application.
CMD ["dotnet", "Gprojet-Backend.dll"]

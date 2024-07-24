#!/bin/sh
set -e

# Ejecuta las migraciones de base de datos
dotnet ef database update --project /app/MagnumServiceApi/MagnumServiceApi.csproj

# Ejecuta la aplicación
dotnet MagnumServiceApi.dll

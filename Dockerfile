# Usa una imagen base de .NET SDK para la construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia los archivos de proyecto y restaura las dependencias
COPY *.sln .
COPY MagnumServiceApi/*.csproj ./MagnumServiceApi/
RUN dotnet restore

# Copia el resto de los archivos del proyecto y construye la aplicación
COPY . .
WORKDIR /app/MagnumServiceApi
RUN dotnet publish -c Release -o out

# Usa una imagen base de .NET Runtime para la ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/MagnumServiceApi/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "MagnumServiceApi.dll"]

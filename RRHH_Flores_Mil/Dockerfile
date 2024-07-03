# Imagen base con SDK de .NET Core para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

# Copiar el archivo de proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto de los archivos y construir la aplicación
COPY . ./
RUN dotnet publish -c Release -o out

# Imagen base mínima para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Exponer el puerto en el que se ejecuta la aplicación ASP.NET Core
EXPOSE 80
ENTRYPOINT ["dotnet", "RRHH_Flores_Mil.dll"]
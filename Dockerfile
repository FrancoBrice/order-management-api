# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copiamos el archivo .csproj y restauramos las dependencias
COPY ["OrderManagement.API/OrderManagement.API.csproj", "OrderManagement.API/"]
RUN dotnet restore "OrderManagement.API/OrderManagement.API.csproj"

# Copiamos el resto del código fuente
COPY . .

WORKDIR "/src/OrderManagement.API"
RUN dotnet publish "OrderManagement.API.csproj" -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderManagement.API.dll"]

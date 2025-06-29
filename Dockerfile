# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar solução e projeto
COPY *.sln .
COPY MotoPack_project/*.csproj ./MotoPack_project/
RUN dotnet restore

# Copiar todo o código, compilar e publicar
COPY . .
WORKDIR /app/MotoPack_project
RUN dotnet publish -c Release -o /out

# Etapa 2: criar imagem final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copiar binário compilado
COPY --from=build /out .

# Copiar a base de dados SQLite
COPY Database/MotoPack.db Database/MotoPack.db

EXPOSE 80

ENTRYPOINT ["dotnet", "MotoPack_project.dll"]

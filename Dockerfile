# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar ficheiros do projeto
COPY *.sln .
COPY MotoPack_project/*.csproj ./MotoPack_project/
RUN dotnet restore

COPY . .
WORKDIR /app/MotoPack_project
RUN dotnet publish -c Release -o /out

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copiar o output da build
COPY --from=build /out .

# Copiar a base de dados (se for usada no runtime)
COPY ./Database/MotoPack.db ./Database/MotoPack.db

# Expõe a porta usada pela app ASP.NET (pode mudar no Program.cs)
EXPOSE 80

# Comando de arranque
ENTRYPOINT ["dotnet", "MotoPack_project.dll"]

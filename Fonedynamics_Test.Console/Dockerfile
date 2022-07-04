#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Fonedynamics_Test.Console/Fonedynamics_Test.Console.csproj", "Fonedynamics_Test.Console/"]
RUN dotnet restore "Fonedynamics_Test.Console/Fonedynamics_Test.Console.csproj"
COPY . .
WORKDIR "/src/Fonedynamics_Test.Console"
RUN dotnet build "Fonedynamics_Test.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Fonedynamics_Test.Console.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fonedynamics_Test.Console.dll"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Server/HVMDash.Server.csproj", "Server/"]
COPY ["Shared/HVMDash.Shared.csproj", "Shared/"]
COPY ["Client/HVMDash.Client.csproj", "Client/"]
COPY ["vkaudioposter-ef/vkaudioposter-ef/vkaudioposter-ef.csproj", "vkaudioposter-ef/vkaudioposter-ef/"]
COPY ["vkaudioposter_Console/vkaudioposter_Console.csproj", "vkaudioposter_Console/"]
RUN dotnet restore "Server/HVMDash.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "HVMDash.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HVMDash.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HVMDash.Server.dll"]
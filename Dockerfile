FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY *.sln ./
COPY ["WebAPI/WebAPI.csproj", "WebAPI/"]
COPY ["BusinessLogic/BusinessLogic.csproj", "BusinessLogic/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["WebAPI.Tests/WebAPI.Tests.csproj", "WebAPI.Tests/"]
RUN dotnet restore
COPY . .
#WORKDIR "/src/."
#RUN dotnet build -c Release -o /app/build


#FROM build AS publish
RUN dotnet publish -c Release -o /app/publish


FROM base AS final
WORKDIR /app
#COPY --from=publish /app/publish .
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebAPI.dll"]





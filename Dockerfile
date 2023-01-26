#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
ARG BUILD_MODE
WORKDIR /vtvlive/src/app
COPY ["DAO/", "DAO/"]
COPY ["LoggerService/", "LoggerService/"]
COPY ["RedisSystem/", "RedisSystem/"]
COPY ["ShareData/", "ShareData/"]
COPY ["ServerEventTet2023/", "ServerEventTet2023/"]
COPY ["UtilsSystem/", "UtilsSystem/"]
COPY ["packages/", "packages/"]

WORKDIR "/vtvlive/src/app/ServerEventTet2023"

#RUN dotnet restore "VtvliveServices.Authen.csproj"

RUN dotnet build "ServerEventTet2023.csproj" -c $BUILD_MODE -o /app/build

FROM build AS publish
ARG BUILD_MODE
RUN dotnet publish "ServerEventTet2023.csproj" -c $BUILD_MODE -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /app/publish .
COPY entrypoint.sh ./entrypoint.sh
RUN apt-get update && apt-get -y install gettext-base
RUN chmod +x ./entrypoint.sh
ENTRYPOINT [ "./entrypoint.sh" ]
CMD ["ServerEventTet2023.dll"]

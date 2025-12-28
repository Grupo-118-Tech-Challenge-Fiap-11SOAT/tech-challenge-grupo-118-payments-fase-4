FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
RUN apk update && apk add wget
RUN apk add --no-cache icu-libs icu-data-full tzdata ttf-opensans
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN ln -s /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime
ENV LC_ALL=pt_BR.UTF-8 \
    LANG=pt_BR.UTF-8

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["Payments.Api/Payments.Api.csproj", "Payments.Api/"]
COPY . .
WORKDIR "/src/Payments.Api"
RUN dotnet build "Payments.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Payments.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Payments.Api.dll"]
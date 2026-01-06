FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
RUN apk update && apk add --no-cache wget icu-libs icu-data-full tzdata ttf-opensans
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN ln -s /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime
ENV LC_ALL=pt_BR.UTF-8 \
    LANG=pt_BR.UTF-8
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["src/Payments.Api/Payments.Api.csproj", "Payments.Api/"]
COPY ["src/Payments.Application/Payments.Application.csproj", "Payments.Application/"]
COPY ["src/Payments.Domain/Payments.Domain.csproj", "Payments.Domain/"]
COPY ["src/Payments.Infra/Payments.Infra.csproj", "Payments.Infra/"]
RUN dotnet restore "Payments.Api/Payments.Api.csproj"
COPY src/ .
WORKDIR "/src/Payments.Api"
RUN dotnet build "Payments.Api.csproj" -c Release -o /app --no-restore

FROM build AS publish
RUN dotnet publish "Payments.Api.csproj" -c Release -o /app --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Payments.Api.dll"]
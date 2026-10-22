# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia os .csproj primeiro (aproveita cache do Docker)
COPY ["Portfolio.Api/Portfolio.Api.csproj", "Portfolio.Api/"]
COPY ["Portfolio.ApiBase/Portfolio.ApiBase.csproj", "Portfolio.ApiBase/"]
COPY ["Portfolio.Aws/Portfolio.Aws.csproj", "Portfolio.Aws/"]
COPY ["Portfolio.Domain/Portfolio.Domain.csproj", "Portfolio.Domain/"]
COPY ["Directory.Packages.props", "."]
COPY ["Portfolio.slnx", "."]

RUN dotnet restore "Portfolio.Api/Portfolio.Api.csproj"

# Copia o resto do código e publica
COPY . .

WORKDIR /src/Portfolio.Api
RUN dotnet publish "Portfolio.Api.csproj" \
    -c Release \
    -r linux-x64 \
    --self-contained false \
    -o /app/publish \
    -p:UseAppHost=false \
    -p:DebugType=none \
    -p:DebugSymbols=false

# ---------- RUNTIME ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

USER app

COPY --from=build --chown=app:app /app/publish .

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

EXPOSE 5000
EXPOSE 5001


ENTRYPOINT ["dotnet", "Portfolio.Api.dll"]
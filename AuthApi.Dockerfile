FROM registry.nexdev.net:5050/docker/dotnet/core/sdk:3.1 AS builder
WORKDIR /src

COPY ./L1 ./L1
COPY ./L1.IdentityServerApi/L1.IdentityServerApi.csproj ./L1.IdentityServerApi/L1.IdentityServerApi.csproj

WORKDIR /src/L1.IdentityServerApi
RUN dotnet restore -v diag

COPY ./L1.IdentityServerApi .

RUN dotnet publish -r linux-x64 --self-contained false -o /app 

FROM registry.nexdev.net:5050/docker/dotnet/core/aspnet:3.1-buster-slim AS base

WORKDIR /app
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "L1.IdentityServerApi.dll"]

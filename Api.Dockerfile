FROM hub.lumigame.com/base/dotnet/core/sdk:5.0.102-ca-patch-buster-slim-2 AS builder
WORKDIR /src

COPY ./L1 ./L1
COPY ./L1.WebApi/L1.WebApi.csproj ./L1.WebApi/L1.WebApi.csproj

WORKDIR /src/L1.WebApi
RUN dotnet restore -v diag

COPY ./L1.WebApi .

RUN dotnet publish -r linux-x64 --self-contained false -o /app 

FROM hub.lumigame.com/base/dotnet/core/aspnet:5.0.15-buster-slim AS base

RUN apt-get update && apt-get install wget curl telnet -y

WORKDIR /app
RUN adduser app_user --disabled-password
USER app_user
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "L1.WebApi.dll"]

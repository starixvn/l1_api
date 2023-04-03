FROM hub.lumigame.com/base/dotnet/core/sdk:5.0.102-ca-patch-buster-slim-2 AS builder
WORKDIR /src

COPY ./L1 ./L1
COPY ./L1.WebApi/L1.WebApi.csproj ./L1.WebApi/L1.WebApi.csproj

WORKDIR /src/L1.WebApi
RUN dotnet restore -v diag

COPY ./L1.WebApi .

RUN dotnet publish -r linux-x64 --self-contained false -o /app 

FROM hub.lumigame.com/base/dotnet/core/aspnet:5.0-buster-slim AS base

WORKDIR /app
ENV APP_USER app_user
RUN useradd -r $APP_USER
USER $APP_USER
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "L1.WebApi.dll"]

FROM hub.lumigame.com/base/dotnet/core/sdk:5.0.102-ca-patch-buster-slim-2 AS builder
WORKDIR /src

COPY ./L1 ./L1
COPY ./L1.IdentityServerApi/L1.IdentityServerApi.csproj ./L1.IdentityServerApi/L1.IdentityServerApi.csproj

WORKDIR /src/L1.IdentityServerApi
RUN dotnet restore -v diag

COPY ./L1.IdentityServerApi .

RUN dotnet publish -r linux-x64 --self-contained false -o /app 

FROM hub.lumigame.com/base/dotnet/core/aspnet:5.0-alpine AS base

WORKDIR /app
ENV APP_USER app_user
RUN adduser -r $APP_USER
USER $APP_USER
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "L1.IdentityServerApi.dll"]

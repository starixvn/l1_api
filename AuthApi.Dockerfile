FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS builder
WORKDIR /src

ARG DEPLOY_ENV=DEV
RUN echo $DEPLOY_ENV

COPY ./L1 ./L1
COPY ./L1.IdentityServerApi ./L1.IdentityServerApi
COPY ./L1.IdentityServerApi/L1.IdentityServerApi.csproj ./L1.IdentityServerApi/L1.IdentityServerApi.csproj

WORKDIR /src/L1.IdentityServerApi
RUN dotnet restore -v diag

COPY ./L1.IdentityServerApi .
RUN dotnet publish -c $DEPLOY_ENV -r linux-x64 --self-contained false -o /app 

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base

WORKDIR /app
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "L1.IdentityServerApi.dll"]
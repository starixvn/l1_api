FROM git3.nexdev.net:5050/lumi-ops/docker-images/dotnet/core/sdk:3.1 AS builder
WORKDIR /src

COPY ./L1 ./L1
COPY ./L1.IdentityServerApi/L1.IdentityServerApi.csproj ./L1.IdentityServerApi/L1.IdentityServerApi.csproj

WORKDIR /src/L1.IdentityServerApi
RUN dotnet restore -v diag

COPY ./L1.IdentityServerApi .

ARG CACHEBUST=1 
ARG DEPLOY_ENV=DEV
RUN echo $DEPLOY_ENV $CACHEBUST

RUN dotnet publish -c $DEPLOY_ENV -r linux-x64 --self-contained false -o /app 

FROM git3.nexdev.net:5050/lumi-ops/docker-images/dotnet/core/aspnet:3.1 AS base

WORKDIR /app
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "L1.IdentityServerApi.dll"]
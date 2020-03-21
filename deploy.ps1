if (-not (Test-Path env:API_COMPILED)) {
  $env:API_COMPILED = "D:\api-compiled"
}

if (!(Test-Path $env:API_COMPILED)) {
  New-Item -ItemType Directory -Force -Path $env:API_COMPILED
}

if ((Get-ChildItem $env:API_COMPILED).Count -eq 0) {
  $CLONE_CMD = "git clone git@gitlab.com:l110/api-compiled.git " + $env:API_COMPILED
  Invoke-Expression $CLONE_CMD
} else {
  $PULL_CMD = "git -C $env:API_COMPILED pull"
  Invoke-Expression $PULL_CMD
}

$WebApi = "L1.WebApi"
$IdentityServerApi = "L1.IdentityServerApi"
$MemberSiteSimulation = "L1.MemberSiteSimulation"
$OWApiSimulator = "OW.ApiSimulator"

# Delete previous compiled
$DELETE_COMPILED_WEBAPI = "Remove-Item -recurse -path $env:API_COMPILED\$WebApi"
if ((Test-Path "$env:API_COMPILED\$WebApi")) {
  Invoke-Expression $DELETE_COMPILED_WEBAPI
}

$DELETE_COMPILED_IDENTITYSERVER = "Remove-Item -recurse -path $env:API_COMPILED\$IdentityServerApi"

if ((Test-Path "$env:API_COMPILED\$IdentityServerApi")) {
  Invoke-Expression $DELETE_COMPILED_IDENTITYSERVER
}

$DELETE_COMPILED_MEMBERSITE = "Remove-Item -recurse -path $env:API_COMPILED\$MemberSiteSimulation"

if ((Test-Path "$env:API_COMPILED\$MemberSiteSimulation")) {
  Invoke-Expression $DELETE_COMPILED_MEMBERSITE
}

$DELETE_COMPILED_OWAPI = "Remove-Item -recurse -path $env:API_COMPILED\$OWApiSimulator"

if ((Test-Path "$env:API_COMPILED\$OWApiSimulator")) {
  Invoke-Expression $DELETE_COMPILED_OWAPI
}


# Compile
$BUILD_WEB_API = "dotnet publish $WebApi -c release -r linux-x64 --self-contained false -o $env:API_COMPILED\api"
$BUILD_IDENTITY_SERVER_API = "dotnet publish $IdentityServerApi -c release -r linux-x64 --self-contained false -o $env:API_COMPILED\identity-api"
$BUILD_MEMBER_SITE = "dotnet publish $MemberSiteSimulation -c release -r linux-x64 --self-contained false -o $env:API_COMPILED\member-site-simulator"
$BUILD_OWAPI_SIMULATOR = "dotnet publish $OWApiSimulator -c release -r linux-x64 --self-contained false -o $env:API_COMPILED\ow-api-simulator"

Invoke-Expression $BUILD_WEB_API
Invoke-Expression $BUILD_IDENTITY_SERVER_API
Invoke-Expression $BUILD_MEMBER_SITE
Invoke-Expression $BUILD_OWAPI_SIMULATOR

# Copy GameLobby
Copy-Item -Path ".\L1.GameLobbySimulator\*" -Destination "$env:API_COMPILED\game-lobby-simulator" -Recurse -Force

# Copy GameClient
Copy-Item -Path ".\L1.Client.NodeJS\*" -Destination "$env:API_COMPILED\L1.Client.NodeJS" -Recurse -Force

# Push git
$CommitMessage = "'deploy-" + (Get-Date -UFormat "%Y%m%d-%H%M%S") + "'"
$PUSH_COMPILED = "cd $env:API_COMPILED; git add .; git commit -m $CommitMessage; git pull --rebase; git push origin master"
Invoke-Expression $PUSH_COMPILED
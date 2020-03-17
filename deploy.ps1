if (-not (Test-Path env:API_COMPILED)) {
  $env:API_COMPILED = "D:\api-compiled"
}

if (!(Test-Path $env:API_COMPILED)) {
  New-Item -ItemType Directory -Force -Path $env:API_COMPILED
}

if ((Get-ChildItem $env:API_COMPILED).Count -eq 0) {
  $CLONE_CMD = "git clone git@gitlab.com:l110/api-compiled.git " + $env:API_COMPILED
  Invoke-Expression $CLONE_CMD
}

$WebApi = "L1.WebApi"
$IdentityServerApi = "L1.IdentityServerApi"

# Delete previous compiled
$DELETE_COMPILED_WEBAPI = "Remove-Item -recurse -path $env:API_COMPILED\$WebApi"
if ((Test-Path "$env:API_COMPILED\$WebApi")) {
  Invoke-Expression $DELETE_COMPILED_WEBAPI
}

$DELETE_COMPILED_IDENTITYSERVER = "Remove-Item -recurse -path $env:API_COMPILED\$IdentityServerApi"

if ((Test-Path "$env:API_COMPILED\$IdentityServerApi")) {
  Invoke-Expression $DELETE_COMPILED_IDENTITYSERVER
}

# Compile
$BUILD_WEB_API = "dotnet publish $WebApi -c release -r linux-x64 --self-contained false -o $env:API_COMPILED\$WebApi"
$BUILD_IDENTITY_SERVER_API = "dotnet publish $IdentityServerApi -c release -r linux-x64 --self-contained false -o $env:API_COMPILED\$IdentityServerApi"

Invoke-Expression $BUILD_WEB_API
Invoke-Expression $BUILD_IDENTITY_SERVER_API

# Push git
$CommitMessage = "'deploy-" + (Get-Date -UFormat "%Y%m%d-%H%M%S") + "'"
$PUSH_COMPILED = "cd $env:API_COMPILED; git add .; git commit -m $CommitMessage; git pull --rebase; git push origin master"
Invoke-Expression $PUSH_COMPILED
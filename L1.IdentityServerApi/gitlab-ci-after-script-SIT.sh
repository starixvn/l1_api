git fetch

HEADHASH=$(git rev-parse HEAD)
UPSTREAMHASH=$(git rev-parse SIT@{upstream})
if [ "$HEADHASH" != "$UPSTREAMHASH" ]; then
  echo "Pull Git"
  git pull;
  echo "Restart service";
  sudo systemctl restart l1-identity-api-sit.service;
else
    echo "Up-to-date";
fi

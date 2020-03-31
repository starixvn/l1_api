git fetch

HEADHASH=$(git rev-parse HEAD)
UPSTREAMHASH=$(git rev-parse UAT@{upstream})
if [ "$HEADHASH" != "$UPSTREAMHASH" ]; then
  echo "Pull Git"
  git checkout -- .;
  git pull;
  echo "Restart service";
  sudo systemctl restart l1-api-uat.service;
else
    echo "Up-to-date";
fi

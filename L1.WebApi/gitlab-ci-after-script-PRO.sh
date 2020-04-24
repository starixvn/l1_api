git fetch

HEADHASH=$(git rev-parse HEAD)
UPSTREAMHASH=$(git rev-parse master@{upstream})
if [ "$HEADHASH" != "$UPSTREAMHASH" ]; then
  echo "Pull Git"
  git checkout -- .;
  git pull;
  echo "Restart service";
  sudo systemctl restart l2-api.service;
else
    echo "Up-to-date";
fi

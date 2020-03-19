const initAuth = require("./auth");

const GameClient = config => {
  if (!GameClient.instance) {
    GameClient.instance = {
      config,
      auth: initAuth(config)
    };
  }

  return GameClient.instance;
};

module.exports = config => {
  const instance = GameClient(config);
  Object.freeze(GameClient);

  return instance;
};

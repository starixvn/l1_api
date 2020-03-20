const initAuth = require("./auth");
const initMember = require("./member");

const GameClient = config => {
  if (!GameClient.instance) {
    GameClient.instance = {
      config,
      auth: initAuth(config),
      member: initMember(config)
    };
  }

  return GameClient.instance;
};

module.exports = config => {
  const instance = GameClient(config);
  
  return instance;
};

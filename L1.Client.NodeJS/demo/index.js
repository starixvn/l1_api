const GameClient = require("../index");

const config = {
    apiUrl: "http://localhost:8868",
  authUrl: "http://auth.lumigame.com",
  client: {
    id: "licensee1",
    secret: "123licensee1%^&sit"
  },
  siteId: "licensee1"
};

GameClient(config);

require("./auth");

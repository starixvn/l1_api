const { SingleSignOnParams } = require("../src/authentication-request");
const AuthInfo = require("../src/authentication-info");
const AuthClient = require("../src/authentication-client");

const authInfo = new AuthInfo(
  "http://api.lumigame.com/",
  "http://auth.lumigame.com",
  "1234321",
  "123licensee1%^&sit",
  "NODE_CLIENT",
  "NODE_SITE"
);
const client = new AuthClient(authInfo);
client.singleSignOnAsync()

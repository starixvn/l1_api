const { SingleSignOnParams } = require("../src/authentication-request");
const AuthInfo = require("../src/authentication-info");
const AuthClient = require("../src/authentication-client");

const authInfo = new AuthInfo(
  "http://api.lumigame.com/",
  "http://auth.lumigame.com/",
  "licensee1",
  "123licensee1%^&sit",
  "",
  "licensee1"
);

var signInParams = new SingleSignOnParams(
  "licensee1",
  12344321,
  "MemberName",
  "en-US",
  "UUS",
  "0.0.0.0",
  "Browser Agent"
);

const client = new AuthClient(authInfo);

client.singleSignOnAsync(signInParams).then(console.log);

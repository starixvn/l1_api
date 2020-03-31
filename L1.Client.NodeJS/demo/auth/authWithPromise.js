const url = require("url");
const GameClient = require("../../index");

const config = {
  apiUrl: "http://api.lumigame.com",
  authUrl: "http://auth.lumigame.com",
  client: {
    id: "game-server",
    secret: "gameserver@20luminet&"
  },
  requestTimeout: 10000,
  useAuthenticate: true
};

const gameClient = GameClient(config);

const singleSignInParams = {
  siteId: "4100300",
  memberId: 12344321,
  memberName: "MemberName",
  language: "en-US",
  currency: "UUS",
  ip: "202.78.231.34",
  browserUserAgent: "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36",
  seq: "d8382c1d-6ce0-433d-9233-e7358b10a70b",
  auth: {
    clientId: "licensee1",
    clientSecret: "123licensee1%^&sit"
  }
};

const withAsyncAwait = async () => {
  await gameClient.autoUpdateToken();
  console.log(gameClient);

  console.log("Starting demo Authentication using async/await");

  // Single Sign On
  console.log("\nSingleSignOn\n");
  try {
    const singleSignOnResult = await gameClient.auth.singleSignOn(
      singleSignInParams
    );
    console.log(singleSignOnResult);
  } catch (error) {
    console.log(`Single Sign On Error: ${error}`);
  }

  // Verify Token
  console.log("\nVerifyToken\n");
  try {
    const redirectUrl = await gameClient.auth.singleSignOn(singleSignInParams, {requestTimeout: 2000});
    const token = url.parse(redirectUrl, true).query.token;
    const params = {
      token,
      browserUserAgent: "Test",
      ip: "0.0.0.0"
    };

    const verifyResult = await gameClient.auth.verifyToken(params);
    console.log(verifyResult);
  } catch (error) {
    console.log(`Verify Token Error: ${error}`);
  }
};

module.exports = withAsyncAwait;

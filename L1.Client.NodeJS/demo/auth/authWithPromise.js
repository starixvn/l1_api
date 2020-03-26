const url = require("url");
const GameClient = require("../../index");

const config = {
  apiUrl: "http://api.lumigame.com",
  authUrl: "http://auth.lumigame.com",
  client: {
    id: "game-server",
    secret: "gameserver@20luminet&"
  }
};

const gameClient = GameClient(config);

const singleSignInParams = {
  siteId: "licensee1",
  memberId: 12344321,
  memberName: "MemberName",
  language: "en-US",
  currency: "UUS",
  ip: "0.0.0.0",
  browserUserAgent: "Browser Agent",
  seq: "d8382c1d-6ce0-433d-9233-e7358b10a70b"
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
    const redirectUrl = await gameClient.auth.singleSignOn(singleSignInParams);
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

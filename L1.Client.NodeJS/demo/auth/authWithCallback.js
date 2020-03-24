const url = require("url");
const GameClient = require("../../index");

const config = {
  apiUrl: "http://api.lumigame.com",
  authUrl: "http://auth.lumigame.com",
  client: {
    id: "licensee1",
    secret: "123licensee1%^&sit"
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

const withCallback = async () => {
  console.log("\n\n\n\n\n\n", gameClient);
  await gameClient.autoUpdateToken();
  console.log("\n\n\n\n\n\n", gameClient);

  console.log("Starting demo Authentication using callback functions");

  // Single Sign On
  console.log("\nSingleSignOn\n");
  gameClient.auth.singleSignOn(
    singleSignInParams,
    (error, singleSignOnResult) => {
      error && console.log(`Single Sign On Error: ${error}`);

      // Verify Token
      const token = url.parse(singleSignOnResult, true).query.token;
      const params = {
        token,
        browserUserAgent: "Test",
        ip: "0.0.0.0"
      };

      gameClient.auth.verifyToken(params, (error, verifyResult) => {
        error && console.log(`Verify Token Error: ${error}`);

        console.log(verifyResult);
      });
    }
  );
};

module.exports = withCallback;

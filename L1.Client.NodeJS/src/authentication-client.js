const { Issuer } = require("openid-client");

const getAccessTokenDefault = async authenticationInfo => {
  const issuer = await Issuer.discover(authenticationInfo.authUrl);
  const Client = issuer.Client;
  const client = new Client(authenticationInfo.getMetadata());
  console.log(client);
  //console.log(issuer);
};

class AuthenticationClient {
  constructor(authenticationInfo, getAccessToken) {
    this.authenticationInfo = authenticationInfo;
    this.getAccessToken = getAccessToken || getAccessTokenDefault;
  }

  async singleSignOnAsync(signOnParams) {
    const tokenResponse = await this.getAccessToken(this.authenticationInfo);

    console.log(tokenResponse);
  }
}

module.exports = AuthenticationClient;

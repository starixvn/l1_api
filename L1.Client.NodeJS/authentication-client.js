const { Issuer } = require("openid-client");
const { SingleSignOnResult } = require("./authentication-response");
const request = require("request");

const getAccessTokenDefault = async authenticationInfo => {
  const issuer = await Issuer.discover(authenticationInfo.authUrl);
  const Client = issuer.Client;
  const client = new Client(authenticationInfo.getMetadata());
  const tokenAccess = await client.requestObject();
  const data = await client.requestResource(`${authenticationInfo.apiUrl}signin/index`, tokenAccess)

  console.log(data.body);
  return tokenAccess;
};

class AuthenticationClient {
  constructor(authenticationInfo, getAccessToken) {
    this.authenticationInfo = authenticationInfo;
    this.getAccessToken = getAccessToken || getAccessTokenDefault;
  }

  async singleSignOnAsync(signOnParams) {
    const token = await this.getAccessToken(this.authenticationInfo);
    console.log(token);
    const url = `${this.authenticationInfo.apiUrl}signin/index`;

    request({
      url,
      method: 'POST',
      auth: {
        bearer: token.substr(0, token.length - 1)
      }
    }, (error, res) => {
      const fs = require("fs");
    });
  }
}

module.exports = AuthenticationClient;

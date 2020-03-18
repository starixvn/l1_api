const { Issuer } = require("openid-client");
const { SingleSignOnResult } = require("./authentication-response");
const Axios = require("axios").default;

const getAccessTokenDefault = async authenticationInfo => {
  const token = await Issuer.discover(authenticationInfo.authUrl)
    .then(issuer => new issuer.Client(authenticationInfo.getMetadata()))
    .then(client =>
      client.grant({
        grant_type: "client_credentials",
        ...authenticationInfo.getMetadata()
      })
    )
    .catch(SingleSignOnResult.authErrorResponse);

  return token;
};

class AuthenticationClient {
  constructor(authenticationInfo, getAccessToken) {
    this.authenticationInfo = authenticationInfo;
    this.getAccessToken = getAccessToken || getAccessTokenDefault;
  }

  async singleSignOnAsync(signOnParams) {
    const token = await this.getAccessToken(this.authenticationInfo);
    const url = `${this.authenticationInfo.apiUrl}signin/index`;

    if (token && token.access_token) {
      const config = {
        headers: { Authorization: `${token.token_type} ${token.access_token}` }
      };

      try {
        const response = await Axios.post(url, signOnParams, config);

        return SingleSignOnResult.createSuccessResponse(response.data);
      } catch (error) {
        return SingleSignOnResult.apiErrorResponse(error);
      }
    }

    return token ? token : SingleSignOnResult.UnknownErrorResponse;
  }
}

module.exports = AuthenticationClient;

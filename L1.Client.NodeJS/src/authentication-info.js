class AuthenticationInfo {
  constructor(apiUrl, authUrl, clientId, clientSecret, siteId) {
    this.apiUrl = apiUrl;
    this.authUrl = authUrl;
    this.clientId = clientId;
    this.clientSecret = clientSecret;
    this.siteId = siteId;
  }

  getMetadata() {
    return {
      client_id: this.clientId,
      client_secret: this.clientSecret,
      site_id: this.siteId
    };
  }
}

module.exports = AuthenticationInfo;

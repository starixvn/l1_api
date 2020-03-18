class SingleSignOnParams {
  constructor(
    siteId,
    memberId,
    memberName,
    language,
    currency,
    ip,
    browserUserAgent,
    seq
  ) {
    this.siteId = siteId;
    this.memberId = memberId;
    this.memberName = memberName;
    this.language = language;
    this.currency = currency;
    this.ip = ip;
    this.browserUserAgent = browserUserAgent;
    this.seq = seq;
  }
}

module.exports = { SingleSignOnParams };

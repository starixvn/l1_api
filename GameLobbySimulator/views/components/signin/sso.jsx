import React from "react";
import Axios from "axios";

class SSOSection extends React.Component {
  constructor() {
    super();

    this.state = {
      memberId: 1111111,
      memberName: "MemberName",
      language: "en-US",
      currency: "UUS",
      seq: "d8382c1d-6ce0-433d-9233-e7358b10a70b",
      siteId: "licensee1",
      ip: "0.0.0.0",
      browserUserAgent: "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_13_6)",
      token: "",
      verifyResult: null
    };
  }

  handleChange(event) {
    const state = this.state;
    state[event.target.id] = event.target.value;

    this.setState(state);
  }

  handleSubmit(event) {
    const request = Object.assign({}, this.state, {
      token: null,
      verifyResult: null
    });
    Axios.post("/signin/sso", request)
      .then(data => data.data)
      .then(token => this.setState(token));

    event.preventDefault();
  }

  handleVerifyToken(event) {
    const request = Object.assign(
      {},
      {
        token: this.state.token,
        browserUserAgent: this.state.browserUserAgent,
        ip: this.state.ip
      }
    );

    Axios.post("/signin/verify", request)
      .then(data => data.data)
      .then(verifyResult => this.setState({ verifyResult }));

    event.preventDefault();
  }

  render() {
    return (
      <>
        <div className="section">
          <form onSubmit={event => this.handleSubmit(event)}>
            <fieldset className="container">
              <legend>Get Token</legend>
              <div className="row">
                <div className="col-sm-12 col-md-6">
                  <div className="input-group vertical">
                    <label htmlFor="memberId">Member ID</label>
                    <input
                      type="number"
                      defaultValue={this.state.memberId}
                      id="memberId"
                      placeholder="Member ID"
                      onChange={event => this.handleChange(event)}
                    />
                  </div>
                  <div className="input-group vertical">
                    <label htmlFor="memberName">Member Username</label>
                    <input
                      type="text"
                      defaultValue={this.state.memberName}
                      id="memberName"
                      placeholder="Member Username"
                      onChange={event => this.handleChange(event)}
                    />
                  </div>
                  <div className="input-group vertical">
                    <label htmlFor="language">Language</label>
                    <select
                      className="input"
                      id="language"
                      onChange={event => this.handleChange(event)}
                      value={this.state.language}
                    >
                      <option value="en-US">English</option>
                      <option value="zh-CN">Chinese Simplified</option>
                      <option value="zh-TW">Chinese Traditional</option>
                    </select>
                  </div>

                  <div className="input-group vertical">
                    <label htmlFor="currency">Currency</label>
                    <select
                      id="currency"
                      onChange={event => this.handleChange(event)}
                      value={this.state.currency}
                    >
                      <option value="$US">$US</option>
                      <option value="IRR">IRR</option>
                      <option value="NTD">NTD</option>
                      <option value="US$">US$</option>
                      <option value="UT">UT</option>
                      <option value="Y$US">Y$US</option>
                      <option value="Yen">Yen</option>
                      <option value="UUS">UUS</option>
                    </select>
                  </div>
                </div>
                <div className="col-sm-12 col-md-6">
                  <div className="input-group vertical">
                    <label htmlFor="seq">SEQ</label>
                    <input
                      type="text"
                      defaultValue={this.state.seq}
                      id="seq"
                      placeholder="seq"
                      onChange={event => this.handleChange(event)}
                    />
                  </div>
                  <div className="input-group vertical">
                    <label htmlFor="ip">Site Id</label>
                    <input
                      type="text"
                      defaultValue={this.state.siteId}
                      id="ip"
                      placeholder="Your ip: xxx.xxx.xxx.xxx"
                      onChange={event => this.handleChange(event)}
                    />
                  </div>
                  <div className="input-group vertical">
                    <label htmlFor="ip">IP</label>
                    <input
                      type="text"
                      defaultValue={this.state.ip}
                      id="ip"
                      placeholder="Your ip: xxx.xxx.xxx.xxx"
                      onChange={event => this.handleChange(event)}
                    />
                  </div>
                  <div className="input-group vertical">
                    <label htmlFor="browserUserAgent">Browser User Agent</label>
                    <input
                      type="text"
                      defaultValue={this.state.browserUserAgent}
                      id="browserUserAgent"
                      placeholder="Your ip: xxx.xxx.xxx.xxx"
                      onChange={event => this.handleChange(event)}
                    />
                  </div>
                </div>
              </div>
              <input className="primary" type="submit" value="Get Token" />
            </fieldset>
          </form>
        </div>
        <div className="section">
          <form onSubmit={event => this.handleVerifyToken(event)}>
            <fieldset className="container">
              <legend>Token</legend>

              <div className="input-group vertical">
                <label htmlFor="token">Token</label>
                <input
                  type="text"
                  value={this.state.token}
                  id="token"
                  placeholder="Your token"
                  onChange={event => this.handleChange(event)}
                />
              </div>
              <input className="primary" type="submit" value="Verify Token" />
            </fieldset>
          </form>
        </div>
        {this.state.verifyResult && (
          <div className="section">
            <form>
              <fieldset className="container">
                <legend>Verify Result</legend>
                <pre>{JSON.stringify(this.state.verifyResult, "<br/>", 2)}</pre>
              </fieldset>
            </form>
          </div>
        )}
      </>
    );
  }
}

export default SSOSection;

import React from "react";
import Axios from "axios";
import { randomString, showAlert } from "../util.jsx";
import Input from "../components/input.jsx";
import LanguageSelect from "../components/select/language.jsx";
import CurrencySelect from "../components/select/currency.jsx";
import GameSelect from "../components/select/game.jsx";

export default class SignIn extends React.Component {
  constructor() {
    super();

    this.state = {
      memberId: (Math.random() * 10000000) >> 0,
      memberName: randomString(8),
      language: "vi-VN",
      currency: "UUS",
      seq: "d8382c1d-6ce0-433d-9233-e7358b10a70b",
      siteId: "licensee1",
      ip: "0.0.0.0",
      browserUserAgent: navigator.userAgent,
      auth: {
        clientId: "licensee1",
        clientSecret: "123licensee1%^&sit"
      },
      token: "",
      redirectUrl: "#",
      verifyResult: null,
      gameId: 1
    };

    this.handleInputChange = this.handleInputChange.bind(this);
    this.handleChangeAuth = this.handleChangeAuth.bind(this);
  }

  handleInputChange(data) {
    this.setState(data);
  }

  handleChangeAuth(data) {
    const state = this.state;

    state.auth = Object.assign({}, state.auth, data);

    this.setState(state);
  }

  handleChange(event) {
    const state = this.state;
    if (event.target.type === "number") {
      state[event.target.id] = parseInt(event.target.value);
    } else {
      state[event.target.id] = event.target.value;
    }

    this.setState(state);
  }

  handleSubmit(event) {
    const request = Object.assign({}, this.state, {
      token: null,
      verifyResult: null,
      resource: null
    });
    Axios.post("/signin/sso", request)
      .then(data => data.data)
      .then(data => (data.error ? Promise.reject(data.error) : data))
      .then(token => this.setState(token))
      .catch(showAlert);

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
      .then(data => (data.error ? Promise.reject(data.error) : data))
      .then(verifyResult => this.setState({ verifyResult }))
      .catch(showAlert);

    event.preventDefault();
  }

  componentDidMount() {
    Axios.get("/resource/currency")
      .then(data => data.data)
      .then(currency =>
        Axios.get("/resource/game")
          .then(data => data.data)
          .then(game => ({
            currency,
            game
          }))
      )
      .then(resource =>
        Axios.get("/resource/language")
          .then(data => data.data)
          .then(language => ({ language, ...resource }))
      )
      .then(resource => this.setState({ resource }));
  }

  render() {
    console.log(this.state);
    return (
      <div className="container">
        <div className="section">
          <form onSubmit={event => this.handleSubmit(event)}>
            <fieldset className="container">
              <legend>Get Token</legend>
              <div className="row">
                <div className="col-sm-12 col-md-6">
                  <Input
                    type="int"
                    id="memberId"
                    name="Member ID"
                    value={this.state.memberId}
                    onChange={this.handleInputChange}
                  />
                  <Input
                    type="text"
                    id="memberName"
                    name="Member Username"
                    value={this.state.memberName}
                    onChange={this.handleInputChange}
                  />
                  <LanguageSelect
                    onChange={this.handleInputChange}
                    id="language"
                    value={this.state.language}
                  />
                  <CurrencySelect
                    onChange={this.handleInputChange}
                    id="currency"
                    value={this.state.currency}
                  />
                  <GameSelect
                    onChange={this.handleInputChange}
                    id="gameId"
                    value={this.state.gameId}
                  />
                  <Input
                    type="text"
                    id="seq"
                    name="SEQ"
                    value={this.state.seq}
                    onChange={this.handleInputChange}
                  />
                </div>
                <div className="col-sm-12 col-md-6">
                  <Input
                    type="text"
                    id="siteId"
                    name="Site Id"
                    value={this.state.seq}
                    onChange={this.handleInputChange}
                  />
                  <Input
                    type="text"
                    id="ip"
                    name="IP"
                    value={this.state.ip}
                    onChange={this.handleInputChange}
                  />
                  <Input
                    type="text"
                    id="browserUserAgent"
                    name="Browser User Agent"
                    value={this.state.browserUserAgent}
                    onChange={this.handleInputChange}
                  />
                  <Input
                    type="text"
                    id="clientId"
                    name="Client ID"
                    value={this.state.auth.clientId}
                    onChange={this.handleChangeAuth}
                  />
                  <Input
                    type="text"
                    id="clientSecret"
                    name="Client Secret"
                    value={this.state.auth.clientSecret}
                    onChange={this.handleChangeAuth}
                  />
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
                <label htmlFor="redirectUrl">Redirect</label>
                <a
                  className="col-sm-12"
                  href={this.state.redirectUrl}
                  target="_blank"
                >
                  {this.state.redirectUrl}
                </a>
              </div>
              <Input
                type="text"
                id="token"
                name="Token"
                value={this.state.token}
                onChange={this.handleInputChange}
              />
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
      </div>
    );
  }
}

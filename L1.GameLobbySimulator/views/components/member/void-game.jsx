import React from "react";
import Axios from "axios";
import { showAlert, randomString } from "../../util.jsx";
import Input from "../input.jsx";
import LanguageSelect from "../select/language.jsx";

export default class VoidGame extends React.Component {
  constructor() {
    super();

    this.state = {
      memberId: (Math.random() * 10000000) >> 0,
      siteId: "licensee1",
      gameRoundId: 1,
      obTransId: (Math.random() * 100) >> 0,
      reason: randomString(8),
      languageCode: "vi-VN",
      result: null
    };

    this.changeValue = this.changeValue.bind(this);
  }

  changeValue(data) {
    this.setState(data);
  }

  handleSubmit(event) {
    const request = Object.assign({}, this.state, {
      result: undefined,
      resource: undefined
    });

    Axios.post("/member/voidGame", request)
      .then(data => data.data)
      .then(data => (data.error ? Promise.reject(data.error) : data))
      .then(result => this.setState({ result }))
      .catch(showAlert);

    event.preventDefault();
  }

  render() {
    return (
      <div className="section">
        <div className="row">
          <div className="col-sm-12 col-md-6">
            <form onSubmit={event => this.handleSubmit(event)}>
              <fieldset className="container">
                <legend>Void game</legend>
                <Input
                  type="int"
                  id="memberId"
                  name="Member ID"
                  value={this.state.memberId}
                  onChange={this.changeValue}
                />
                <Input
                  type="text"
                  id="siteId"
                  name="Site ID"
                  value={this.state.siteId}
                  onChange={this.changeValue}
                />
                <Input
                  type="int"
                  id="gameRoundId"
                  name="Game Round ID"
                  value={this.state.gameRoundId}
                  onChange={this.changeValue}
                />
                <Input
                  type="int"
                  id="obTransId"
                  name="ObTrans ID"
                  value={this.state.obTransId}
                  onChange={this.changeValue}
                />
                <Input
                  type="text"
                  id="reason"
                  name="Reason"
                  value={this.state.reason}
                  onChange={this.changeValue}
                />
                <LanguageSelect
                  onChange={this.changeValue}
                  id="languageCode"
                  value={this.state.languageCode}
                />
                <input className="primary" type="submit" value="End" />
              </fieldset>
            </form>
          </div>
          <div className="col-sm-12 col-md-6">
            <fieldset className="container">
              <legend>Void game result</legend>
              <pre>{JSON.stringify(this.state.result, null, 2)}</pre>
            </fieldset>
          </div>
        </div>
      </div>
    );
  }
}

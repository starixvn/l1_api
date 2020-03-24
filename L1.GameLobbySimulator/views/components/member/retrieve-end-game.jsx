import React from "react";
import Axios from "axios";
import { showAlert } from "../../util.jsx";
import Input from "../input.jsx";
import LanguageSelect from "../select/language.jsx";
import GameSelect from "../select/game.jsx";

export default class RetriveEndGame extends React.Component {
  constructor() {
    super();

    this.state = {
      siteId: "licensee1",
      gameRoundId: 1,
      gameTypeId: 1,
      obCustId: (Math.random() * 100) >> 0,
      seq: "d8382c1d-6ce0-433d-9233-e7358b10a70b",
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

    Axios.post("/member/retrieveEndGameInfo", request)
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
                <legend>Retrieve End-Game Info</legend>
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
                <GameSelect
                  onChange={this.changeValue}
                  id="gameTypeId"
                  value={this.state.gameTypeId}
                />
                <Input
                  type="int"
                  id="obCustId"
                  name="ObCustId"
                  value={this.state.obCustId}
                  onChange={this.changeValue}
                />
                <Input
                  type="text"
                  id="seq"
                  name="SEQ"
                  value={this.state.seq}
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
              <legend>Retrieve Result</legend>
              <pre>{JSON.stringify(this.state.result, null, 2)}</pre>
            </fieldset>
          </div>
        </div>
      </div>
    );
  }
}

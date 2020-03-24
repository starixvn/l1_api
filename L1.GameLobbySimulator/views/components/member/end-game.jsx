import React from "react";
import Axios from "axios";
import { showAlert } from "../../util.jsx";
import Input from "../input.jsx";
import LanguageSelect from "../select/language.jsx";
import GameSelect from "../select/game.jsx";

export default class EndGame extends React.Component {
  constructor() {
    super();

    this.state = {
      memberId: (Math.random() * 10000000) >> 0,
      siteId: "licensee1",
      gameRoundId: 1,
      gameTypeId: 1,
      totalAmount: (Math.random() * 100) >> 0,
      totalWin: (Math.random() * 100) >> 0,
      validBetAmount: (Math.random() * 100) >> 0,
      languageCode: "vi-VN",
      endTime: new Date().toISOString(),
      result: null,
      resource: {
        game: []
      }
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

    Axios.post("/member/endGame", request)
      .then(data => data.data)
      .then(data => (data.error ? Promise.reject(data.error) : data))
      .then(result => this.setState({ result }))
      .catch(showAlert);

    event.preventDefault();
  }

  componentDidMount() {
    Axios.get("/resource/game")
      .then(data => data.data)
      .then(game => ({ game }))
      .then(resource => this.setState({ resource }));
  }

  render() {
    console.log(this.state.endTime);
    return (
      <div className="section">
        <div className="row">
          <div className="col-sm-12 col-md-6">
            <form onSubmit={event => this.handleSubmit(event)}>
              <fieldset className="container">
                <legend>End game</legend>
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
                <GameSelect
                  onChange={this.changeValue}
                  id="gameTypeId"
                  value={this.state.gameTypeId}
                />
                <Input
                  type="float"
                  id="totalAmount"
                  name="Total Amount"
                  value={this.state.totalAmount}
                  step="0.01"
                  onChange={this.changeValue}
                />
                <Input
                  type="int"
                  id="totalWin"
                  name="Total Win"
                  value={this.state.totalWin}
                  onChange={this.changeValue}
                />
                <Input
                  type="int"
                  id="validBetAmount"
                  name="Valid bet amount"
                  value={this.state.validBetAmount}
                  onChange={this.changeValue}
                />
                <Input
                  type="datetime-local"
                  id="endTime"
                  name="End Time"
                  value={this.state.endTime.substring(
                    0,
                    this.state.endTime.length - 1
                  )}
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
              <legend>End game result</legend>
              <pre>{JSON.stringify(this.state.result, null, 2)}</pre>
            </fieldset>
          </div>
        </div>
      </div>
    );
  }
}

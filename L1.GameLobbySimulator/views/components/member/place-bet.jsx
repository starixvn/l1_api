import React from "react";
import Axios from "axios";
import { showAlert } from "../../util.jsx";
import Input from "../input.jsx";
import LanguageSelect from "../select/language.jsx";

export default class PlaceBet extends React.Component {
  constructor() {
    super();

    this.state = {
      memberId: (Math.random() * 10000000) >> 0,
      siteId: "licensee1",
      gameRoundId: 1,
      gameTypeId: 1,
      choiceId: 1,
      amount: (Math.random() * 100) >> 0,
      currency: "UUS",
      ip: "0.0.0.0",
      languageCode: "vi-VN",
      result: null,
      resource: {
        game: [
          {
            id: 1,
            choice: []
          }
        ],
        currency: []
      }
    };

    this.changeValue = this.changeValue.bind(this);
  }

  changeValue(data) {
    this.setState(data);
  }

  handleChange(event) {
    const state = this.state;
    if (event.target.id === "gameTypeId") {
      state.choiceId = 1;
      state[event.target.id] = parseInt(event.target.value);
    } else if (event.target.id === "choiceId") {
      state[event.target.id] = parseInt(event.target.value);
    } else {
      state[event.target.id] = event.target.value;
    }

    this.setState(state);
  }

  handleSubmit(event) {
    const request = Object.assign({}, this.state, {
      result: undefined,
      resource: undefined
    });

    Axios.post("/member/placeBet", request)
      .then(data => data.data)
      .then(data => (data.error ? Promise.reject(data.error) : data))
      .then(result => this.setState({ result }))
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
      .then(resource => this.setState({ resource }));
  }

  render() {
    return (
      <div className="section">
        <div className="row">
          <div className="col-sm-12 col-md-6">
            <form onSubmit={event => this.handleSubmit(event)}>
              <fieldset className="container">
                <legend>Bet</legend>
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
                <div className="input-group vertical">
                  <label htmlFor="currency">Currency</label>
                  <select
                    id="currency"
                    onChange={event => this.handleChange(event)}
                    value={this.state.currency}
                  >
                    {this.state.resource.currency.map(curr => (
                      <option value={curr.value} key={curr.value}>
                        {curr.name}
                      </option>
                    ))}
                  </select>
                </div>

                <div className="input-group vertical">
                  <label htmlFor="gameTypeId">Game</label>
                  <select
                    id="gameTypeId"
                    onChange={event => this.handleChange(event)}
                    value={this.state.gameId}
                  >
                    {this.state.resource.game.map(game => (
                      <option value={game.id} key={game.id}>
                        {game.name}
                      </option>
                    ))}
                  </select>
                </div>

                <div className="input-group vertical">
                  <label htmlFor="choiceId">Choice</label>
                  <select
                    id="choiceId"
                    onChange={event => this.handleChange(event)}
                    value={this.state.choiceId}
                  >
                    {this.state.resource.game[
                      this.state.gameTypeId - 1
                    ].choice.map(choice => (
                      <option
                        value={choice.id}
                        key={`${this.state.gameTypeId}${choice.id}`}
                      >
                        {choice.name}
                      </option>
                    ))}
                  </select>
                </div>

                <Input
                  type="float"
                  id="amount"
                  name="Amount"
                  step="0.01"
                  value={this.state.amount}
                  onChange={this.changeValue}
                />
                <Input
                  type="text"
                  id="ip"
                  name="IP"
                  value={this.state.ip}
                  onChange={this.changeValue}
                />
                <LanguageSelect
                  onChange={this.changeValue}
                  id="languageCode"
                  value={this.state.languageCode}
                />
                <input className="primary" type="submit" value="Bet" />
              </fieldset>
            </form>
          </div>
          <div className="col-sm-12 col-md-6">
            <fieldset className="container">
              <legend>Bet Result</legend>
              <pre>{JSON.stringify(this.state.result, null, 2)}</pre>
            </fieldset>
          </div>
        </div>
      </div>
    );
  }
}

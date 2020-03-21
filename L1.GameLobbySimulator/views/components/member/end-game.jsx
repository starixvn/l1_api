import React from "react";
import Axios from "axios";
import { showAlert } from "../../util.jsx";

export default class EndGame extends React.Component {
  constructor() {
    super();

    this.state = {
      memberId: (Math.random() * 10000000) >> 0,
      seq: "d8382c1d-6ce0-433d-9233-e7358b10a70b",
      siteId: "licensee1",
      gameRoundId: 1,
      gameTypeId: 1,
      totalAmount: (Math.random() * 100) >> 0,
      totalWin: (Math.random() * 100) >> 0,
      validBetAmount: (Math.random() * 100) >> 0,
      result: null,
      resource: {
        game: []
      }
    };
  }

  handleChange(event) {
    const state = this.state;
    if (event.target.id === "gameTypeId") {
      state[event.target.id] = parseInt(event.target.value);
    } else if (event.target.type === "number") {
      if (event.target.id === "totalAmount") {
        state[event.target.id] = parseFloat(event.target.value);
      } else {
        state[event.target.id] = parseInt(event.target.value);
      }
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
    return (
      <div className="section">
        <div className="row">
          <div className="col-sm-12 col-md-6">
            <form onSubmit={event => this.handleSubmit(event)}>
              <fieldset className="container">
                <legend>End game</legend>
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
                  <label htmlFor="siteId">Site ID</label>
                  <input
                    type="text"
                    defaultValue={this.state.siteId}
                    id="siteId"
                    placeholder="Site ID"
                    onChange={event => this.handleChange(event)}
                  />
                </div>
                <div className="input-group vertical">
                  <label htmlFor="gameRoundId">Game Round ID</label>
                  <input
                    type="number"
                    defaultValue={this.state.gameRoundId}
                    id="gameRoundId"
                    placeholder="Game round id"
                    onChange={event => this.handleChange(event)}
                  />
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
                  <label htmlFor="totalAmount">Total Amount</label>
                  <input
                    type="number"
                    defaultValue={this.state.totalAmount}
                    id="totalAmount"
                    placeholder="total amount"
                    step="0.01"
                    onChange={event => this.handleChange(event)}
                  />
                </div>
                <div className="input-group vertical">
                  <label htmlFor="totalWin">Total Win</label>
                  <input
                    type="number"
                    defaultValue={this.state.totalWin}
                    id="totalWin"
                    placeholder="total win"
                    onChange={event => this.handleChange(event)}
                  />
                </div>
                <div className="input-group vertical">
                  <label htmlFor="validBetAmount">Valid bet amount</label>
                  <input
                    type="number"
                    defaultValue={this.state.validBetAmount}
                    id="validBetAmount"
                    placeholder="valid bet amount"
                    onChange={event => this.handleChange(event)}
                  />
                </div>
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

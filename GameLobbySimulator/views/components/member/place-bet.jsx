import React from "react";
import Axios from "axios";
import { showAlert } from "../../util.jsx";

export default class Balance extends React.Component {
  constructor() {
    super();

    this.state = {
      memberId: (Math.random() * 10000000) >> 0,
      seq: "d8382c1d-6ce0-433d-9233-e7358b10a70b",
      siteId: "licensee1",
      gameRoundId: 1,
      gameTypeId: 1,
      choiceId: 1,
      amount: (Math.random() * 1000) >> 0,
      currency: "UUS",
      ip: "0.0.0.0",
      result: null,
      resource: {
        game: []
      }
    };
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
      result: undefined
    });

    Axios.post("/member/balance", request)
      .then(data => data.data)
      .then(data => (data.error ? Promise.reject(data.error) : data))
      .then(result => this.setState({ result }))
      .catch(showAlert);

    event.preventDefault();
  }

  componentDidMount() {
    Axios.get("/resource/game")
      .then(data => data.data)
      .then(game => {
        const state = this.state;
        state.resource.game = game;

        this.setState(state);
      });
  }

  render() {
    return (
      <div className="section">
        <div className="row">
          <div className="col-sm-12 col-md-6">
            <form onSubmit={event => this.handleSubmit(event)}>
              <fieldset className="container">
                <legend>Bet</legend>
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

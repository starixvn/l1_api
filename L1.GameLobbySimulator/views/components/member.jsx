import React from "react";
import Axios from "axios";
import { randomString, showAlert } from "../util.jsx";

export default class Member extends React.Component {
  constructor() {
    super();

    this.state = {
      memberId: (Math.random() * 10000000) >> 0,
      characterName: randomString(8),
      seq: "d8382c1d-6ce0-433d-9233-e7358b10a70b",
      siteId: "licensee1",
      firstLogin: false,
      result: null
    };
  }

  handleChange(event) {
    const state = this.state;

    if (event.target.type === "checkbox") {
      state[event.target.id] = event.target.checked;
    } else if (event.target.type === 'number') {
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

    Axios.post("/member/notify", request)
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
                <legend>Notify Login</legend>

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
                  <label htmlFor="characterName">Character name</label>
                  <input
                    type="text"
                    defaultValue={this.state.characterName}
                    id="characterName"
                    placeholder="Character Name"
                    onChange={event => this.handleChange(event)}
                  />
                </div>
                <div className="row responsive-label">
                  <div className="col-sm-12 col-md-3">
                    <label htmlFor="firstLogin">First Login</label>
                  </div>
                  <div className="col-sm-12 col-md">
                    <input
                      type="checkbox"
                      checked={this.props.firstLogin}
                      id="firstLogin"
                      onChange={event => this.handleChange(event)}
                    />
                  </div>
                </div>
                <input className="primary" type="submit" value="Notify" />
              </fieldset>
            </form>
          </div>
          <div className="col-sm-12 col-md-6">
            <fieldset className="container">
              <legend>Notify Result</legend>
              <pre>{JSON.stringify(this.state.result, null, 2)}</pre>
            </fieldset>
          </div>
        </div>
      </div>
    );
  }
}

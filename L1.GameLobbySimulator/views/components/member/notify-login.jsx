import React from "react";
import Axios from "axios";
import { randomString, showAlert } from "../../util.jsx";
import Input from "../input.jsx";

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

    this.handleChange = this.handleChange.bind(this);
  }

  handleChange(data) {
    this.setState(data);
  }

  handleSubmit(event) {
    const request = Object.assign({}, this.state, {
      result: undefined
    });

    Axios.post("/member/notifyLogin", request)
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
                <Input
                  type="int"
                  id="memberId"
                  name="Member ID"
                  value={this.state.memberId}
                  onChange={this.handleChange}
                />
                <Input
                  type="text"
                  id="characterName"
                  name="Chacter Name"
                  value={this.state.characterName}
                  onChange={this.handleChange}
                />
                <Input
                  type="text"
                  id="seq"
                  name="SEQ"
                  value={this.state.seq}
                  onChange={this.handleChange}
                />
                <Input
                  type="text"
                  id="siteId"
                  name="Site ID"
                  value={this.state.siteId}
                  onChange={this.handleChange}
                />
                <Input
                  type="checkbox"
                  id="firstLogin"
                  name="First Login"
                  value={this.state.siteId}
                  onChange={this.handleChange}
                />
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

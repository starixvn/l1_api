import React from "react";
import Axios from "axios";
import { showAlert } from "../../util.jsx";
import Input from "../input.jsx";

export default class GameClient extends React.Component {
  constructor() {
    super();

    this.state = {
      apiUrl: "",
      authUrl: "",
      client: {
        id: "",
        secret: ""
      }
    };

    this.onChangeValue = this.onChangeValue.bind(this);
    this.onChangeClient = this.onChangeClient.bind(this);
  }

  componentDidMount() {
    Axios.get("/setting/data")
      .then(data => data.data)
      .then(data => this.setState(data));
  }

  onChangeValue(data) {
    this.setState(data);
  }

  onChangeClient(data) {
    const state = this.state;
    state.client = Object.assign({}, state.client, data);

    this.setState(data);
  }

  handleSubmit(event) {
    Axios.post("/setting/data", this.state)
      .then(data => data.data)
      .then(data => (data.error ? Promise.reject(data.error) : data))
      .then(data => this.setState(data))
      .catch(showAlert);

    event.preventDefault();
  }

  render() {
    return (
      <div className="section">
        <form onSubmit={event => this.handleSubmit(event)}>
          <legend>General</legend>
          <div className="row">
            <div className="col-sm-12 col-md-6">
              <fieldset className="container">
                <legend>Url</legend>
                <Input
                  type="text"
                  id="apiUrl"
                  name="API Url"
                  value={this.state.apiUrl}
                  onChange={this.onChangeValue}
                />
                <Input
                  type="text"
                  id="authUrl"
                  name="Auth URL"
                  value={this.state.authUrl}
                  onChange={this.onChangeValue}
                />
              </fieldset>
            </div>
            <div className="col-sm-12 col-md-6">
              <fieldset className="container">
                <legend>Client</legend>
                <Input
                  type="text"
                  id="id"
                  name="Id"
                  value={this.state.client.id}
                  onChange={this.onChangeClient}
                />
                <Input
                  type="text"
                  id="secret"
                  name="Secret"
                  value={this.state.client.secret}
                  onChange={this.onChangeClient}
                />
              </fieldset>
            </div>
          </div>
          <input className="primary" type="submit" value="Update" />
        </form>
      </div>
    );
  }
}

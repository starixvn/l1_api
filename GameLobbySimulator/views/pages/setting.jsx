import React from "react";
import Axios from "axios";
import { showAlert } from "../util.jsx";

class SettingPage extends React.Component {
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
  }

  componentDidMount() {
    Axios.get("/setting/data")
      .then(data => data.data)
      .then(data => this.setState(data));
  }

  handleChange(event) {
    const state = this.state;
    const id = event.target.id;

    const splited = id.split(".");

    if (splited.length === 2) {
      state[splited[0]][splited[1]] = event.target.value;
    } else {
      state[event.target.id] = event.target.value;
    }

    this.setState(state);
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
      <div className="container">
        <div className="section">
          <form onSubmit={event => this.handleSubmit(event)}>
            <div className="row">
              <div className="col-sm-12 col-md-6">
                <fieldset className="container">
                  <legend>Url</legend>

                  <div className="input-group vertical">
                    <label htmlFor="apiUrl">API Url</label>
                    <input
                      type="text"
                      defaultValue={this.state.apiUrl}
                      id="apiUrl"
                      placeholder="API Url"
                      onChange={event => this.handleChange(event)}
                    />
                  </div>
                  <div className="input-group vertical">
                    <label htmlFor="authUrl">Auth Url</label>
                    <input
                      type="text"
                      defaultValue={this.state.authUrl}
                      id="authUrl"
                      placeholder="API Url"
                      onChange={event => this.handleChange(event)}
                    />
                  </div>
                </fieldset>
              </div>
              <div className="col-sm-12 col-md-6">
                <fieldset className="container">
                  <legend>Client</legend>

                  <div className="input-group vertical">
                    <label htmlFor="client.id">Id</label>
                    <input
                      type="text"
                      defaultValue={this.state.client.id}
                      id="client.id"
                      placeholder="Client ID"
                      onChange={event => this.handleChange(event)}
                    />
                  </div>
                  <div className="input-group vertical">
                    <label htmlFor="client.secret">Secret</label>
                    <input
                      type="text"
                      defaultValue={this.state.client.secret}
                      id="client.secret"
                      placeholder="Client Secret"
                      onChange={event => this.handleChange(event)}
                    />
                  </div>
                </fieldset>
              </div>
            </div>
            <input className="primary" type="submit" value="Update" />
          </form>
        </div>
      </div>
    );
  }
}

export default SettingPage;

import React from "react";
import Axios from "axios";
import { showAlert } from "../../util.jsx";
import Input from "../input.jsx";

export default class GameClient extends React.Component {
  constructor() {
    super();

    this.state = {
      language: [],
      newLanguage: ""
    };

    this.onChangeValue = this.onChangeValue.bind(this);
  }

  componentDidMount() {
    Axios.get("/resource/language")
      .then(data => data.data)
      .then(language => this.setState({ language }));
  }

  onChangeValue(data) {
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
          <div className="row">
            <div className="col-sm-12 col-md-6">
              <fieldset className="container">
                <legend>Language</legend>
                <Input
                  type="text"
                  id="newLanguage"
                  name="<value>:<name>"
                  value={this.state.newLanguage}
                  onChange={this.onChangeValue}
                />
              </fieldset>
            </div>
            <div className="col-sm-12 col-md-6">
              <fieldset className="container">
                <legend>Supported</legend>
                <ul>
                  {this.state.language.map(language => {
                    return (
                      <li
                        key={language.value}
                      >{`${language.name} (${language.value})`}</li>
                    );
                  })}
                </ul>
              </fieldset>
            </div>
          </div>
          <input className="primary" type="submit" value="Add" />
        </form>
      </div>
    );
  }
}

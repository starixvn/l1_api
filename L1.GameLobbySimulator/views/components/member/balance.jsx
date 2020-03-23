import React from "react";
import Axios from "axios";
import { showAlert } from "../../util.jsx";
import Input from "../input.jsx";
import LanguageSelect from "../select/language.jsx";

export default class Balance extends React.Component {
  constructor() {
    super();

    this.state = {
      memberId: (Math.random() * 10000000) >> 0,
      siteId: "licensee1",
      languageCode: "vi-VN",
      result: null
    };

    this.handleChange = this.handleChange.bind(this);
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

  handleChange(data) {
    this.setState(data);
  }

  render() {
    return (
      <div className="section">
        <div className="row">
          <div className="col-sm-12 col-md-6">
            <form onSubmit={event => this.handleSubmit(event)}>
              <fieldset className="container">
                <legend>Balance</legend>
                <Input
                  type="int"
                  id="memberId"
                  name="Member ID"
                  value={this.state.memberId}
                  onChange={this.handleChange}
                />
                <Input
                  type="text"
                  id="siteId"
                  name="Site ID"
                  value={this.state.siteId}
                  onChange={this.handleChange}
                />
                <LanguageSelect
                  onChange={this.handleChange}
                  id="languageCode"
                  value={this.state.languageCode}
                />
                <input className="primary" type="submit" value="Balance" />
              </fieldset>
            </form>
          </div>
          <div className="col-sm-12 col-md-6">
            <fieldset className="container">
              <legend>Balance Result</legend>
              <pre>{JSON.stringify(this.state.result, null, 2)}</pre>
            </fieldset>
          </div>
        </div>
      </div>
    );
  }
}

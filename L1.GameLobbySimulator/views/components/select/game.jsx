import React from "react";
import Select from "./select.jsx";
import Axios from "axios";

export default class Language extends React.Component {
  constructor(props) {
    super(props);

    this.handleChange = this.handleChange.bind(this);

    this.state = {
      name: "Game",
      type: "int",
      options: []
    };
  }

  handleChange(data) {
    this.props.onChange(data);
  }

  componentDidMount() {
    Axios.get("/resource/game")
      .then(data => data.data)
      .then(options => this.setState({ options }));
  }

  render() {
    const { value, id } = this.props;
    return (
      <Select
        {...this.state}
        id={id}
        value={value}
        onChange={this.handleChange}
      />
    );
  }
}

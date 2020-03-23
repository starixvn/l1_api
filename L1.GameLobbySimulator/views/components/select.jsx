import React from "react";

export default class Input extends React.Component {
  constructor(props) {
    super(props);

    this.handleChange = this.handleChange.bind(this);
  }

  handleChange(event) {
    let value = null;

    if (event.target.type === "number") {
      if (this.props.type === "float") {
        value = parseFloat(event.target.value);
      } else {
        value = parseInt(event.target.value);
      }
    } else {
      value = event.target.value;
    }

    const data = { [this.props.id]: value };

    this.props.onChange(data);
  }

  render() {
    const { id, name, value, type, options } = this.props;
    return (
      <div className="input-group vertical">
        <label htmlFor={id}>{name}</label>
        <select
          className="input"
          type={type}
          id={id}
          onChange={event => this.handleChange(event)}
          value={value}
        >
          {options.map(option => (
            <option value={option.value} key={option.value}>
              {option.name}
            </option>
          ))}
        </select>
      </div>
    );
  }
}

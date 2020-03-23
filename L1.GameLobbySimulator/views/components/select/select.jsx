import React from "react";

export default class Select extends React.Component {
  constructor(props) {
    super(props);

    this.handleChange = this.handleChange.bind(this);
  }

  handleChange(event) {
    let value = null;

    if (event.target.attributes.getNamedItem("type").value === "number") {
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

  getType(typeName) {
    if (typeName === "int" || typeName === "float") {
      return "number";
    }

    return typeName;
  }

  render() {
    const { id, name, value, type, options } = this.props;
    return (
      <div className="input-group vertical">
        <label htmlFor={id}>{name}</label>
        <select
          className="input"
          type={this.getType(type)}
          id={id}
          onChange={event => this.handleChange(event)}
          value={value}
        >
          {options.map(option => (
            <option value={option.value || option.id} key={option.value || option.id}>
              {option.name}
            </option>
          ))}
        </select>
      </div>
    );
  }
}

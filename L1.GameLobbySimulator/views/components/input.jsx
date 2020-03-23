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
    } else if (event.target.type === "checkbox") {
      value = event.target.checked;
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
    const { id, name, type, value } = this.props;
    return (
      <div className="input-group vertical">
        <label htmlFor={id}>{name}</label>
        <input
          type={this.getType(type)}
          defaultValue={value}
          id={id}
          onChange={this.handleChange}
        />
      </div>
    );
  }
}

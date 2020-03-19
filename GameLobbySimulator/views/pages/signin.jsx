import React from "react";

class SignInPage extends React.Component {
  render() {
    return (
      <div className="tabs stacked">
        <input type="radio" name="tab-group" id="tab1" aria-hidden="true" />
        <label htmlFor="tab1" aria-hidden="true">
          Tab 1
        </label>
        <div>
          <h3>Tab 1</h3>
          <p>This is the first tab's content.</p>
        </div>
        <input type="radio" name="tab-group" id="tab2" aria-hidden="true" />
        <label htmlFor="tab2" aria-hidden="true">
          Tab 2
        </label>
        <div>
          <h3>Tab 2</h3>
          <p>This is the second tab's content.</p>
        </div>
        <input type="radio" name="tab-group" id="tab3" aria-hidden="true" />
        <label htmlFor="tab3" aria-hidden="true">
          Tab 3
        </label>
        <div>
          <h3>Tab 3</h3>
          <p>This is the third tab's content.</p>
        </div>
      </div>
    );
  }
}

export default SignInPage;

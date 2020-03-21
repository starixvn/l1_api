import React from "react";
import ReactDOM from "react-dom";
import { createBrowserHistory } from "history";
import { Router, Switch, Route, Redirect } from "react-router-dom";
import routes from "./routes.jsx";
import Header from "./components/header.jsx";

const history = createBrowserHistory();

class Application extends React.Component {
  render() {
    return (
      <Router history={history}>
        <Header />
        <Switch>
          {routes.map(({ path, component }, key) => (
            <Route
              key={key}
              path={path}
              render={props => React.createElement(component, props)}
            />
          ))}
          <Redirect to={routes[0].path} />
        </Switch>
      </Router>
    );
  }
}

ReactDOM.render(<Application />, document.getElementById("app"));

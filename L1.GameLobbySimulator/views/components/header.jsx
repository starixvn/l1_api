import React from "react";
import routes from "../routes.jsx";

const Header = () => (
  <header className="sticky">
    {routes.map(({ path, name }) => (
      <a href={path} className="button" key={path}>
        {name}
      </a>
    ))}
  </header>
);

export default Header;

import React from "react";
import GameClient from "../components/setting/game-client.jsx";
import Language from "../components/setting/language.jsx";

class SettingPage extends React.Component {
  render() {
    return (
      <div className="container">
        <GameClient />
        <Language />
      </div>
    );
  }
}

export default SettingPage;

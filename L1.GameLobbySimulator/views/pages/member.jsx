import React from "react";
import NotifyLogin from "../components/member/notify-login.jsx";
import Balance from "../components/member/balance.jsx";
import PlaceBet from "../components/member/place-bet.jsx";
import EndGame from "../components/member/end-game.jsx";
import VoidGame from "../components/member/void-game.jsx";
import RetrieveEndGame from "../components/member/retrieve-end-game.jsx";

class MemberPage extends React.Component {
  render() {
    return (
      <div className="container">
        <NotifyLogin />
        <Balance />
        <PlaceBet />
        <EndGame />
        <VoidGame />
        <RetrieveEndGame />
      </div>
    );
  }
}

export default MemberPage;

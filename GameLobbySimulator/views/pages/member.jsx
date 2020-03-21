import React from "react";
import NotifyLogin from "../components/member/notify-login.jsx";
import Balance from "../components/member/balance.jsx";
import PlaceBet from "../components/member/place-bet.jsx";

class MemberPage extends React.Component {
  render() {
    return (
      <div className="container">
        <NotifyLogin />
        <Balance />
        
      </div>
    );
  }
}

export default MemberPage;

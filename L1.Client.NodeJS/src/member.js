const Axios = require("axios").default;
const { handleError, handleSuccess } = require("./util");

const notifyLogin = ({ apiUrl }) => (params, callback) =>
  new Promise((resolve, reject) => {
    Axios.post(`${apiUrl}/Member/NotifyMemberLogin`, params)
      .then(data => data.data)
      .then(handleSuccess(resolve, callback))
      .catch(handleError(reject, callback));
  });

module.exports = config => ({
  notifyLogin: notifyLogin(config)
});

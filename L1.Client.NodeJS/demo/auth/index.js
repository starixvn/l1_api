const withPromise = require("./authWithPromise");
const withCallback = require("./authWithCallback");

(async () => {
    await withPromise();
    //withCallback();
})();
const GameClient = require("../../index")();
const url = require("url");

const singleSignInParams = {
    siteId: "licensee1",
    memberId: 12344321,
    memberName: "MemberName",
    language: "en-US",
    currency: "UUS",
    ip: "0.0.0.0",
    browserUserAgent: "Browser Agent"
};

const withAsyncAwait = async () => {
    console.log("Starting demo Authentication using async/await");

    // Single Sign On
    console.log("\nSingleSignOn\n");
    try {
        const singleSignOnResult = await GameClient.auth.singleSignOn(singleSignInParams);
        console.log(singleSignOnResult);
    } catch (error) {
        console.log(`Single Sign On Error: ${error}`);
    }

    // Verify Token
    console.log("\nVerifyToken\n");
    try {
        const redirectUrl = await GameClient.auth.singleSignOn(singleSignInParams);
        const token = url.parse(redirectUrl, true).query.token;
        const params = {
            token,
            browserUserAgent: "Test",
            ip: "0.0.0.0"
        };

        const verifyResult = await GameClient.auth.verifyToken(params);
        console.log(verifyResult);
    } catch (error) {
        console.log(`Verify Token Error: ${error}`);
    }
}

module.exports = withAsyncAwait;
// Common test cases
pm.test("Status code is 200", () => {
    pm.response.to.have.status(200);
});

pm.test("Response has valid structure", () => {
    const jsonData = pm.response.json();
    pm.expect(jsonData).to.have.property('result');
    pm.expect(jsonData.result).to.have.property('errorCode');
    pm.expect(jsonData.result).to.have.property('errorMessage');
});

pm.test("Error code is 0 for success", () => {
    const jsonData = pm.response.json();
    pm.expect(jsonData.result.errorCode).to.equal(0);
}); 
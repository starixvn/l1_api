class SingleSignOnResult {
  constructor(code, message, redirectUrl) {
    this.code = code;
    this.message = message;
    this.redirectUrl = redirectUrl;
  }

  isSuccessful() {
    return this.code === SingleSignOnResult.SUCCESS_CODE;
  }
}

SingleSignOnResult.SUCCESS_CODE = 0;
SingleSignOnResult.CLIENT_ID_DOES_NOT_EXIST_CODE = 0;
SingleSignOnResult.WRONG_CLIENT_SECRET_CODE = 0;
SingleSignOnResult.API_SERVER_NOT_EXIST_CODE = 0;
SingleSignOnResult.IDENTITY_SERVER_NOT_EXIST_CODE = 0;
SingleSignOnResult.SERVER_ERROR_CODE = 0;

SingleSignOnResult.createSuccessResponse = redirectUrl =>
  new SingleSignOnResult(
    SingleSignOnResult.SUCCESS_CODE,
    "",
    new URL(redirectUrl)
  );

SingleSignOnResult.creatErrorResponse = code =>
  new SingleSignOnResult(code, "");

SingleSignOnResult.serverErrorResult = new SingleSignOnResult(
  SingleSignOnResult.SERVER_ERROR_CODE,
  ""
);

module.exports = { SingleSignOnResult };

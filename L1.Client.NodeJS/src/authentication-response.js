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
SingleSignOnResult.AUTH_ERROR_CODE = 1;
SingleSignOnResult.API_ERROR_CODE = 2;
SingleSignOnResult.UNKNOWN_ERROR_CODE = 2;

SingleSignOnResult.createSuccessResponse = redirectUrl =>
  new SingleSignOnResult(
    SingleSignOnResult.SUCCESS_CODE,
    "",
    new URL(redirectUrl)
  );

SingleSignOnResult.apiErrorResponse = error =>
  new SingleSignOnResult(SingleSignOnResult.API_ERROR_CODE, error);

SingleSignOnResult.authErrorResponse = error =>
  new SingleSignOnResult(SingleSignOnResult.AUTH_ERROR_CODE, error);

SingleSignOnResult.UnknownErrorResponse = new SingleSignOnResult(
  SingleSignOnResult.UNKNOWN_ERROR_CODE,
  ""
);

module.exports = { SingleSignOnResult };

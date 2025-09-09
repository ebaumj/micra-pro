namespace MicraPro.Auth.Domain.ValueObjects;

internal record FailedResponseBody : ResponseBody<FailedResponseBody>
{
    public string error { get; }
    public string error_description { get; }

    private FailedResponseBody(string error, string error_description)
    {
        this.error = error;
        this.error_description = error_description;
    }

    public static readonly string InvalidGrantResponse = new FailedResponseBody(
        "invalid_grant",
        "username or password not valid"
    );
    public static readonly string TokenInvalidResponse = new FailedResponseBody(
        "invalid_request",
        "token not valid"
    );
    public static readonly string InvalidRequestResponse = new FailedResponseBody(
        "invalid_request",
        "invalid request"
    );
    public static readonly string UnsupportedGrantTypeResponse = new FailedResponseBody(
        "unsupported_grant_type",
        "only password and refresh_token grants are supported"
    );
}

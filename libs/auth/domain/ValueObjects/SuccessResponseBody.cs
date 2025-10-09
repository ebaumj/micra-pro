namespace MicraPro.Auth.Domain.ValueObjects;

internal record SuccessResponseBody : ResponseBody<SuccessResponseBody>
{
    public string access_token { get; }
    public string refresh_token { get; }
    public string token_type { get; }
    public int expires_in { get; }

    private SuccessResponseBody(
        string access_token,
        string refresh_token,
        string token_type,
        int expires_in
    )
    {
        this.access_token = access_token;
        this.refresh_token = refresh_token;
        this.token_type = token_type;
        this.expires_in = expires_in;
    }

    public static string Create(
        string accessToken,
        string refreshToken,
        string jwtTokenLifeTimeInMinutes
    ) =>
        new SuccessResponseBody(
            accessToken,
            refreshToken,
            "bearer",
            (int)Math.Floor(double.Parse(jwtTokenLifeTimeInMinutes) * 60)
        );
}

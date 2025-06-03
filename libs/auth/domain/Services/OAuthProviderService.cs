using System.Net;
using System.Text.Json;
using MicraPro.Auth.DataDefinition;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace MicraPro.Auth.Domain.Services;

public class OAuthProviderService(
    IJwtCreatorService jwtCreatorService,
    IAccountManagementService accountManagementService,
    IOptions<AuthDomainOptions> jwtConfig
) : IOAuthProviderService
{
    private static Dictionary<string, string> ReadUrlEncodedBody(string httpBody)
    {
        Dictionary<string, string> result = new();
        foreach (var encodedKeyValuePair in httpBody.Split('&'))
        {
            var decodedKey = WebUtility.UrlDecode(encodedKeyValuePair.Split('=')[0]);
            var decodedValue = WebUtility.UrlDecode(encodedKeyValuePair.Split('=')[1]);
            result.Add(decodedKey, decodedValue);
        }
        if (!result.TryGetValue("grant_type", out var value))
            throw new InvalidDataException("grant_type not provided!");
        return value switch
        {
            "password" when !result.ContainsKey("username") => throw new InvalidDataException(
                "username not provided!"
            ),
            "password" when !result.ContainsKey("password") => throw new InvalidDataException(
                "password not provided!"
            ),
            "password" when !result.ContainsKey("client_id") => throw new InvalidDataException(
                "client_id not provided!"
            ),
            "refresh_token" when !result.ContainsKey("refresh_token") =>
                throw new InvalidDataException("refresh_token not provided!"),
            _ => result,
        };
    }

    private record SuccessResponseBodyType
    {
        public string access_token { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;
        public string token_type { get; set; } = "bearer";
        public int expires_in { get; set; }
    }

    private record FailedResponseBodyType
    {
        public string error { get; set; } = string.Empty;
        public string error_description { get; set; } = string.Empty;
    }

    private string CreateResponse(string accessToken, string refreshToken)
    {
        return JsonSerializer.Serialize(
            new SuccessResponseBodyType()
            {
                access_token = accessToken,
                expires_in = (int)
                    Math.Floor(double.Parse(jwtConfig.Value.JwtTokenLifeTimeInMinutes) * 60),
                refresh_token = refreshToken,
            }
        );
    }

    private string CreateWrongGrantTypeResponse()
    {
        return JsonSerializer.Serialize(
            new FailedResponseBodyType
            {
                error = "unsupported_grant_type",
                error_description = "only password and refresh_token grants are supported",
            }
        );
    }

    private string CreateWrongPasswordResponse()
    {
        return JsonSerializer.Serialize(
            new FailedResponseBodyType
            {
                error = "invalid_grant",
                error_description = "username or password not valid",
            }
        );
    }

    private string CreateUnauthorizedResponse()
    {
        return JsonSerializer.Serialize(
            new FailedResponseBodyType
            {
                error = "invalid_request",
                error_description = "token not valid",
            }
        );
    }

    private string CreateFailedParseResponse(string message)
    {
        return JsonSerializer.Serialize(
            new FailedResponseBodyType { error = "invalid_request", error_description = message }
        );
    }

    public async Task<string> HandleAuthRequestAsync(HttpRequest request)
    {
        try
        {
            var requestParameters = ReadUrlEncodedBody(
                await new StreamReader(request.Body).ReadToEndAsync()
            );
            switch (requestParameters["grant_type"])
            {
                case "password":
                    if (
                        !accountManagementService.CheckPassword(
                            requestParameters["username"],
                            requestParameters["password"]
                        )
                    )
                        return CreateWrongPasswordResponse();
                    var user = requestParameters["username"];
                    return CreateResponse(
                        jwtCreatorService.CreateAccessToken(
                            user,
                            accountManagementService.GetRolesForUser(user)
                        ),
                        jwtCreatorService.CreateRefreshToken(
                            user,
                            accountManagementService.GetRolesForUser(user)
                        )
                    );
                case "refresh_token"
                    when !jwtCreatorService.ValidateRefreshToken(
                        requestParameters["refresh_token"]
                    ):
                    return CreateUnauthorizedResponse();
                case "refresh_token":
                {
                    var username = jwtCreatorService.GetUsernameFromRefreshToken(
                        requestParameters["refresh_token"]
                    );
                    var roles = jwtCreatorService
                        .GetPersistedRolesFromRefreshToken(requestParameters["refresh_token"])
                        .ToArray();
                    return CreateResponse(
                        jwtCreatorService.CreateAccessToken(username, roles),
                        jwtCreatorService.CreateRefreshToken(username, roles)
                    );
                }
                default:
                    return CreateWrongGrantTypeResponse();
            }
        }
        catch (InvalidDataException e)
        {
            return CreateFailedParseResponse(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return CreateFailedParseResponse("invalid request");
        }
    }
}

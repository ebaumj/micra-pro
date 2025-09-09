using MicraPro.Auth.DataDefinition;
using MicraPro.Auth.Domain.Interfaces;
using MicraPro.Auth.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace MicraPro.Auth.Domain.Services;

public class OAuthProviderService(
    IJwtCreatorService jwtCreatorService,
    IAccountManagementService accountManagementService,
    IOptions<AuthDomainOptions> jwtConfig
) : IOAuthProviderService
{
    public async Task<string> HandleAuthRequestAsync(HttpRequest request)
    {
        try
        {
            var requestParameters = (
                await new FormReader(
                    await new StreamReader(request.Body).ReadToEndAsync()
                ).ReadFormAsync()
            ).ToDictionary(kv => kv.Key, kv => kv.Value.FirstOrDefault() ?? "");
            switch (requestParameters["grant_type"])
            {
                case "password":
                    if (
                        !accountManagementService.CheckPassword(
                            requestParameters["username"],
                            requestParameters["password"]
                        )
                    )
                        return FailedResponseBody.InvalidGrantResponse;
                    var user = requestParameters["username"];
                    return SuccessResponseBody.Create(
                        jwtCreatorService.CreateAccessToken(
                            user,
                            accountManagementService.GetRolesForUser(user)
                        ),
                        jwtCreatorService.CreateRefreshToken(
                            user,
                            accountManagementService.GetRolesForUser(user)
                        ),
                        jwtConfig.Value.JwtTokenLifeTimeInMinutes
                    );
                case "refresh_token":
                {
                    var refreshToken = requestParameters["refresh_token"];
                    if (!jwtCreatorService.ValidateRefreshToken(refreshToken))
                        return FailedResponseBody.TokenInvalidResponse;
                    var username = jwtCreatorService.GetUsernameFromRefreshToken(refreshToken);
                    var roles = jwtCreatorService
                        .GetPersistedRolesFromRefreshToken(refreshToken)
                        .ToArray();
                    return SuccessResponseBody.Create(
                        jwtCreatorService.CreateAccessToken(username, roles),
                        jwtCreatorService.CreateRefreshToken(username, roles),
                        jwtConfig.Value.JwtTokenLifeTimeInMinutes
                    );
                }
                default:
                    return FailedResponseBody.InvalidRequestResponse;
            }
        }
        catch (Exception)
        {
            return FailedResponseBody.UnsupportedGrantTypeResponse;
        }
    }
}

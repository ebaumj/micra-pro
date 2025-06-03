using HotChocolate.Authorization;
using HotChocolate.Resolvers;
using MicraPro.Auth.DataDefinition;

namespace MicraPro.Auth.DataProvider;

public class AuthorizePermissionHandler(IAuthorizationService<Permission> authorizationService)
    : IAuthorizationHandler
{
    public ValueTask<AuthorizeResult> AuthorizeAsync(
        IMiddlewareContext context,
        AuthorizeDirective directive,
        CancellationToken cancellationToken = new()
    ) => ValueTask.FromResult(Authorize(context.ContextData, directive));

    public ValueTask<AuthorizeResult> AuthorizeAsync(
        AuthorizationContext context,
        IReadOnlyList<AuthorizeDirective> directives,
        CancellationToken cancellationToken = new()
    ) =>
        ValueTask.FromResult(
            directives.Any(directive =>
                Authorize(context.ContextData, directive) == AuthorizeResult.Allowed
            )
                ? AuthorizeResult.Allowed
                : AuthorizeResult.NotAllowed
        );

    private AuthorizeResult Authorize(
        IDictionary<string, object?> contextData,
        AuthorizeDirective directive
    )
    {
        var userState = GetUserState(contextData);
        var requiredPermissions = directive.Roles?.Select(int.Parse).Cast<Permission>();
        if (requiredPermissions == null)
            return AuthorizeResult.NotAllowed;
        return authorizationService.HasPermissions(userState.User, requiredPermissions)
            ? AuthorizeResult.Allowed
            : AuthorizeResult.NotAllowed;
    }

    private static UserState GetUserState(IDictionary<string, object?> contextData)
    {
        if (
            contextData.TryGetValue(WellKnownContextData.UserState, out var value)
            && value is UserState p
        )
        {
            return p;
        }

        throw new MissingStateException(
            "Authorization",
            WellKnownContextData.UserState,
            StateKind.Global
        );
    }
}

using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.AspNetCore.Subscriptions.Protocols;
using MicraPro.Auth.Domain.Interfaces;
using MicraPro.Auth.Domain.Services;

namespace MicraPro.Auth.DataProvider;

public class AuthSocketInterceptor(IJwtCreatorService jwtCreatorService)
    : DefaultSocketSessionInterceptor
{
    private class SocketConnectPayload
    {
        public string? Authorization { get; set; }
    }

    public override async ValueTask<ConnectionStatus> OnConnectAsync(
        ISocketSession session,
        IOperationMessagePayload connectionInitMessage,
        CancellationToken cancellationToken = new()
    )
    {
        var claims = jwtCreatorService.ValidateAccessToken(
            connectionInitMessage
                .As<SocketConnectPayload>()
                ?.Authorization?.Replace("Bearer ", "")
                .Trim() ?? string.Empty
        );
        if (claims == null)
            return ConnectionStatus.Reject("Authentication failed!");
        session.Connection.HttpContext.User = claims;
        return await base.OnConnectAsync(session, connectionInitMessage, cancellationToken);
    }
}

using System.Reflection;
using HotChocolate.Authorization;
using HotChocolate.Types.Descriptors;

namespace MicraPro.Auth.DataDefinition;

public class RequiredPermissionsAttribute(Permission[]? permissions) : AuthorizeAttribute
{
    public const string AuthorizePolicyName = "AuthorizePermission";

    protected override void TryConfigure(
        IDescriptorContext context,
        IDescriptor descriptor,
        ICustomAttributeProvider element
    )
    {
        if (permissions == null)
            return;
        switch (descriptor)
        {
            case IObjectTypeDescriptor type:
                type.Directive(CreateDirective());
                break;
            case IObjectFieldDescriptor field:
                if (Apply is ApplyPolicy.Validation)
                    field.Extend().Context.ContextData[
                        WellKnownContextData.AuthorizationRequestPolicy
                    ] = true;
                field.Directive(CreateDirective());
                break;
        }
    }

    private AuthorizeDirective CreateDirective() =>
        new(
            AuthorizePolicyName,
            permissions?.Cast<int>().Select(p => p.ToString()).ToArray(),
            apply: Apply
        );
}

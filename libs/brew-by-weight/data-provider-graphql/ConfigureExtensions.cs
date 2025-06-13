using System.Linq.Expressions;
using System.Reflection;
using HotChocolate.Execution.Configuration;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.BrewByWeight.DataProviderGraphQl;

public static class ConfigureExtensions
{
    private static Action<IRequestExecutorBuilder> CreateAddBrewByWeightTrackingTypeAction(
        Type type
    ) =>
        builder =>
        {
            typeof(SchemaRequestExecutorBuilderExtensions)
                .GetMethods()
                .FirstOrDefault(m =>
                    m.Name == "AddObjectType"
                    && m.GetParameters().Length == 2
                    && m is { IsGenericMethod: true, IsStatic: true }
                )!
                .MakeGenericMethod(type)
                .Invoke(
                    null,
                    [
                        builder,
                        (object descriptor) =>
                        {
                            typeof(IObjectTypeDescriptor<>)
                                .MakeGenericType(type)
                                .GetMethods()
                                .FirstOrDefault(m =>
                                    m.Name == "Implements" && m.GetParameters().Length == 0
                                )!
                                .MakeGenericMethod(typeof(InterfaceType<BrewByWeightTracking>))
                                .Invoke(descriptor, []);
                            typeof(IObjectTypeDescriptor<>)
                                .MakeGenericType(type)
                                .GetMethods()
                                .FirstOrDefault(m =>
                                    m.Name == "Name" && m.GetParameters().Length == 1
                                )!
                                .Invoke(descriptor, [$"BrewProcess{type.Name}"]);
                            var fieldDescriptor = (IObjectFieldDescriptor)
                                typeof(IObjectTypeDescriptor<>)
                                    .MakeGenericType(type)
                                    .GetMethods()
                                    .FirstOrDefault(m =>
                                        m.Name == "Field"
                                        && m.GetParameters().Length == 1
                                        && m.GetParameters()[0].ParameterType == typeof(MemberInfo)
                                    )!
                                    .Invoke(descriptor, [type.GetMember("ToString").First()])!;
                            fieldDescriptor.Name("_stringValue");
                        },
                    ]
                );
        };

    private static Action<IRequestExecutorBuilder> CreateAddBrewByWeightExceptionTypeAction(
        Type type
    ) =>
        builder =>
        {
            typeof(SchemaRequestExecutorBuilderExtensions)
                .GetMethods()
                .FirstOrDefault(m =>
                    m.Name == "AddObjectType"
                    && m.GetParameters().Length == 2
                    && m is { IsGenericMethod: true, IsStatic: true }
                )!
                .MakeGenericMethod(type)
                .Invoke(
                    null,
                    [
                        builder,
                        (object descriptor) =>
                        {
                            Expression<Func<BrewByWeightException, string>> exp = m => m.Message;
                            typeof(IObjectTypeDescriptor<>)
                                .MakeGenericType(type)
                                .GetMethods()
                                .FirstOrDefault(m =>
                                    m.Name == "Implements" && m.GetParameters().Length == 0
                                )!
                                .MakeGenericMethod(typeof(InterfaceType<BrewByWeightException>))
                                .Invoke(descriptor, []);
                            typeof(IObjectTypeDescriptor<>)
                                .MakeGenericType(type)
                                .GetMethods()
                                .FirstOrDefault(m =>
                                    m.Name == "BindFieldsExplicitly"
                                    && m.GetParameters().Length == 0
                                )!
                                .Invoke(descriptor, []);
                            typeof(IObjectTypeDescriptor<>)
                                .MakeGenericType(type)
                                .GetMethods()
                                .FirstOrDefault(m =>
                                    m.Name == "Field"
                                    && m.GetParameters().Length == 1
                                    && m.GetParameters()[0].ParameterType == typeof(MemberInfo)
                                )!
                                .Invoke(descriptor, [type.GetMember("Message").First()]);
                        },
                    ]
                );
        };

    private static readonly IEnumerable<Action<IRequestExecutorBuilder>> BrewByWeightTrackingTypes =
        typeof(BrewByWeightTracking)
            .Assembly.GetTypes()
            .Where(a => a.IsSubclassOf(typeof(BrewByWeightTracking)))
            .Select(CreateAddBrewByWeightTrackingTypeAction);

    private static readonly IEnumerable<
        Action<IRequestExecutorBuilder>
    > BrewByWeightExceptionTypes = typeof(BrewByWeightException)
        .Assembly.GetTypes()
        .Where(a => a.IsSubclassOf(typeof(BrewByWeightException)))
        .Select(CreateAddBrewByWeightExceptionTypeAction);

    private static IRequestExecutorBuilder ConfigureBrewByWeightTrackingType(
        this IRequestExecutorBuilder builder
    )
    {
        builder.AddInterfaceType<BrewByWeightTracking>(d =>
            d.Field(t => t.ToString()).Name("_stringValue")
        );
        foreach (var addTypeAction in BrewByWeightTrackingTypes)
            addTypeAction(builder);
        builder.AddInterfaceType<BrewByWeightException>(d =>
            d.BindFieldsExplicitly().Field(t => t.Message)
        );
        foreach (var addTypeAction in BrewByWeightExceptionTypes)
            addTypeAction(builder);
        return builder;
    }

    public static IRequestExecutorBuilder AddBrewByWeightDataProviderGraphQlTypes(
        this IRequestExecutorBuilder builder
    )
    {
        return builder.AddDataProviderGraphQlTypes().ConfigureBrewByWeightTrackingType();
    }

    public static IServiceCollection AddBrewByWeightDataProviderGraphQlServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services.AddSingleton<BrewProcessContainerService>();
    }
}

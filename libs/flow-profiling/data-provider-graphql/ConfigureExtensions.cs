using System.Linq.Expressions;
using System.Reflection;
using HotChocolate.Execution.Configuration;
using MicraPro.FlowProfiling.DataDefinition.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.FlowProfiling.DataProviderGraphQl;

public static class ConfigureExtensions
{
    private static Action<IRequestExecutorBuilder> CreateAddFlowProfilingTrackingTypeAction(
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
                                .MakeGenericMethod(typeof(InterfaceType<FlowProfileTracking>))
                                .Invoke(descriptor, []);
                            typeof(IObjectTypeDescriptor<>)
                                .MakeGenericType(type)
                                .GetMethods()
                                .FirstOrDefault(m =>
                                    m.Name == "Name" && m.GetParameters().Length == 1
                                )!
                                .Invoke(descriptor, [$"FlowProfilingProcess{type.Name}"]);
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

    private static Action<IRequestExecutorBuilder> CreateAddFlowProfilingExceptionTypeAction(
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
                            Expression<Func<FlowProfileException, string>> exp = m => m.Message;
                            typeof(IObjectTypeDescriptor<>)
                                .MakeGenericType(type)
                                .GetMethods()
                                .FirstOrDefault(m =>
                                    m.Name == "Implements" && m.GetParameters().Length == 0
                                )!
                                .MakeGenericMethod(typeof(InterfaceType<FlowProfileException>))
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

    private static readonly IEnumerable<
        Action<IRequestExecutorBuilder>
    > FlowProfilingTrackingTypes = typeof(FlowProfileTracking)
        .Assembly.GetTypes()
        .Where(a => a.IsSubclassOf(typeof(FlowProfileTracking)))
        .Select(CreateAddFlowProfilingTrackingTypeAction);

    private static readonly IEnumerable<
        Action<IRequestExecutorBuilder>
    > FlowProfilingExceptionTypes = typeof(FlowProfileException)
        .Assembly.GetTypes()
        .Where(a => a.IsSubclassOf(typeof(FlowProfileException)))
        .Select(CreateAddFlowProfilingExceptionTypeAction);

    private static IRequestExecutorBuilder ConfigureFlowProfilingTrackingType(
        this IRequestExecutorBuilder builder
    )
    {
        builder.AddInterfaceType<FlowProfileTracking>(d =>
            d.Field(t => t.ToString()).Name("_stringValue")
        );
        foreach (var addTypeAction in FlowProfilingTrackingTypes)
            addTypeAction(builder);
        builder.AddInterfaceType<FlowProfileException>(d =>
            d.BindFieldsExplicitly().Field(t => t.Message)
        );
        foreach (var addTypeAction in FlowProfilingExceptionTypes)
            addTypeAction(builder);
        return builder;
    }

    public static IRequestExecutorBuilder AddFlowProfilingDataProviderGraphQlTypes(
        this IRequestExecutorBuilder builder
    )
    {
        return builder.AddDataProviderGraphQlTypes().ConfigureFlowProfilingTrackingType();
    }

    public static IServiceCollection AddFlowProfilingDataProviderGraphQlServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services.AddSingleton<FlowProfilingProcessContainerService>();
    }
}

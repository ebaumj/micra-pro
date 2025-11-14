using System.Reflection;
using HotChocolate.Execution.Configuration;
using MicraPro.Cleaning.DataDefinition.ValueObjects;
using MicraPro.Cleaning.DataProviderGraphQl.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.Cleaning.DataProviderGraphQl;

public static class ConfigureExtensions
{
    private static Action<IRequestExecutorBuilder> CreateAddCleaningStateTypeAction(Type type) =>
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
                                .MakeGenericMethod(typeof(InterfaceType<CleaningState>))
                                .Invoke(descriptor, []);
                            typeof(IObjectTypeDescriptor<>)
                                .MakeGenericType(type)
                                .GetMethods()
                                .FirstOrDefault(m =>
                                    m.Name == "Name" && m.GetParameters().Length == 1
                                )!
                                .Invoke(descriptor, [$"CleaningState{type.Name}"]);
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

    private static readonly IEnumerable<Action<IRequestExecutorBuilder>> CleaningStateTypes =
        typeof(CleaningState)
            .Assembly.GetTypes()
            .Where(a => a.IsSubclassOf(typeof(CleaningState)))
            .Select(CreateAddCleaningStateTypeAction);

    public static IRequestExecutorBuilder AddCleaningDataProviderGraphQlTypes(
        this IRequestExecutorBuilder builder
    )
    {
        builder.AddInterfaceType<CleaningState>(d =>
            d.Field(t => t.ToString()).Name("_stringValue")
        );
        foreach (var addTypeAction in CleaningStateTypes)
            addTypeAction(builder);
        return builder.AddDataProviderGraphQlTypes();
    }

    public static IServiceCollection AddCleaningDataProviderGraphQlServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services.AddSingleton<CleaningProcessContainerService>();
    }
}

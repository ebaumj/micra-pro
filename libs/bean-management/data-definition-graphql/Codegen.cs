using MicraPro.BeanManagement.DataProviderGraphQl;
using MicraPro.Shared.DataProviderGraphQl.Test;
using Path = System.IO.Path;

namespace MicraPro.BeanManagement.DataDefinitionGraphQl;

public class Codegen
{
    [Fact]
    public async Task GenerateSchemaTest()
    {
        var projectDir = Path.Join(Directory.GetCurrentDirectory(), "../../..");
        var schemaPath = Path.Join(projectDir, "src/generated/schema.graphqls");

        var schema = await SchemaBaseTests.GenerateSchemaTest(builder =>
            builder.AddBeanManagementDataProviderGraphQlTypes()
        );

        var schemaDir = Path.GetDirectoryName(schemaPath);
        if (schemaDir is not null)
            Directory.CreateDirectory(schemaDir);
        await File.WriteAllTextAsync(schemaPath, schema);
    }
}

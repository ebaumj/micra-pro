using MicraPro.BrewByWeight.DataProviderGraphQl;
using MicraPro.Shared.DataProviderGraphQl.Test;
using Path = System.IO.Path;

namespace MicraPro.BrewByWeight.DataDefinitionGraphQl;

public class Codegen()
{
    [Fact]
    public async Task GenerateSchemaTest()
    {
        var projectDir = Path.Join(Directory.GetCurrentDirectory(), "../../..");
        var schemaPath = Path.Join(projectDir, "src/generated/schema.graphqls");

        var schema = await SchemaBaseTests.GenerateSchemaTest(builder =>
            builder.AddBrewByWeightDataProviderGraphQlTypes()
        );

        var schemaDir = Path.GetDirectoryName(schemaPath);
        if (schemaDir is not null)
            Directory.CreateDirectory(schemaDir);
        await File.WriteAllTextAsync(schemaPath, schema);
    }
}

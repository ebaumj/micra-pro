using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;

namespace MicraPro.Backend.Test;

internal static class ConfigMicraProBackendMock
{
    private class MockObject : IConfigurationSection
    {
        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return [];
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public IConfigurationSection GetSection(string key)
        {
            throw new NotImplementedException();
        }

        public string? this[string key]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public string Key => String.Empty;
        public string Path => String.Empty;
        public string? Value { get; set; }

        private readonly BackendOptions _options = new() { IncludeExceptionDetails = true };

        public BackendOptions Get<T>() => _options;
    }

    public static IConfiguration Create()
    {
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(m => m.GetSection(BackendOptions.SectionName)).Returns(new MockObject());
        return configMock.Object;
    }
}

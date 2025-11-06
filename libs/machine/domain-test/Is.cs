using Moq;

namespace MicraPro.Machine.Domain.Test;

internal static class Is
{
    public static CancellationToken Ct => It.IsAny<CancellationToken>();

    public static object Obj(object obj) =>
        It.Is<object>(o => o.GetHashCode() == obj.GetHashCode());
}

using MicraPro.Cleaning.Domain.Services;
using Moq;

namespace MicraPro.Cleaning.Domain.Test.Services;

public class CleaningStateServiceTest
{
    [Fact]
    public void StateServiceTest()
    {
        var observerMock = new Mock<IObserver<bool>>();
        var service = new CleaningStateService();
        service.IsRunningObservable.Subscribe(observerMock.Object);
        observerMock.Verify(o => o.OnNext(false), Times.Once);
        Assert.False(service.IsRunning);
        service.SetIsRunning(true);
        observerMock.Verify(o => o.OnNext(true), Times.Once);
        Assert.True(service.IsRunning);
        service.SetIsRunning(false);
        observerMock.Verify(o => o.OnNext(false), Times.Exactly(2));
        Assert.False(service.IsRunning);
        service.SetIsRunning(false);
        observerMock.Verify(o => o.OnNext(false), Times.Exactly(2));
        Assert.False(service.IsRunning);
        observerMock.VerifyNoOtherCalls();
    }
}

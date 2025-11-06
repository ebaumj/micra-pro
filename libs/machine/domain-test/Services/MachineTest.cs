using System.Reactive.Subjects;
using MicraPro.Machine.DataDefinition.ValueObjects;
using MicraPro.Machine.Domain.Interfaces;
using Moq;

namespace MicraPro.Machine.Domain.Test.Services;

public class MachineTest
{
    [Fact]
    public void AllSmartStandbyModesCoveredTest()
    {
        Assert.True(
            Enum.GetValues<SmartStandby.SmartStandbyMode>()
                .All(v => Machine.Domain.Services.Machine.StandbyModeDictionaryTest.Contains(v))
        );
    }

    [Fact]
    public void IsStandbyTest()
    {
        var subject = new BehaviorSubject<bool>(false);
        var machineConnectionMock = new Mock<IMachineConnection>();
        machineConnectionMock.Setup(m => m.IsStandby).Returns(subject);
        var observerMock = new Mock<IObserver<bool>>();
        var service = new Domain.Services.Machine(machineConnectionMock.Object);
        service.IsStandby.Subscribe(observerMock.Object);
        observerMock.Verify(o => o.OnNext(false), Times.Once);
        subject.OnNext(true);
        observerMock.Verify(o => o.OnNext(true), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task SetStandbyAsyncTest(bool standby)
    {
        var machineConnectionMock = new Mock<IMachineConnection>();
        var service = new Domain.Services.Machine(machineConnectionMock.Object);
        await service.SetStandbyAsync(standby, CancellationToken.None);
        if (standby)
            machineConnectionMock.Verify(
                m =>
                    m.WriteValueAsync("MachineChangeMode", Is.Obj(new { mode = "StandBy" }), Is.Ct),
                Times.Once
            );
        else
            machineConnectionMock.Verify(
                m =>
                    m.WriteValueAsync(
                        "MachineChangeMode",
                        Is.Obj(new { mode = "BrewingMode" }),
                        Is.Ct
                    ),
                Times.Once
            );
    }

    [Fact]
    public async Task GetSmartStandbyAsyncTest()
    {
        var machineConnectionMock = new Mock<IMachineConnection>();
        var service = new Domain.Services.Machine(machineConnectionMock.Object);
        machineConnectionMock
            .Setup(m => m.ReadValueAsync(IMachineConnection.ReadSetting.SmartStandBy, Is.Ct))
            .Returns(Task.FromResult("{\"minutes\":15,\"mode\":\"LastBrewing\",\"enabled\":true}"));
        var standby = await service.GetSmartStandbyAsync(CancellationToken.None);
        Assert.NotNull(standby);
        Assert.Equal(TimeSpan.FromMinutes(15), standby.Time);
        Assert.Equal(SmartStandby.SmartStandbyMode.LastBrew, standby.Mode);
        machineConnectionMock
            .Setup(m => m.ReadValueAsync(IMachineConnection.ReadSetting.SmartStandBy, Is.Ct))
            .Returns(Task.FromResult("{\"minutes\":60,\"mode\":\"PowerOn\",\"enabled\":true}"));
        standby = await service.GetSmartStandbyAsync(CancellationToken.None);
        Assert.NotNull(standby);
        Assert.Equal(TimeSpan.FromHours(1), standby.Time);
        Assert.Equal(SmartStandby.SmartStandbyMode.PowerOn, standby.Mode);
        machineConnectionMock
            .Setup(m => m.ReadValueAsync(IMachineConnection.ReadSetting.SmartStandBy, Is.Ct))
            .Returns(Task.FromResult("{\"minutes\":60,\"mode\":\"PowerOn\",\"enabled\":false}"));
        standby = await service.GetSmartStandbyAsync(CancellationToken.None);
        Assert.Null(standby);
    }

    public static readonly TheoryData<SmartStandby> SmartStandbyTestData = new()
    {
        new SmartStandby(TimeSpan.FromHours(1), SmartStandby.SmartStandbyMode.LastBrew),
        new SmartStandby(TimeSpan.FromMinutes(10), SmartStandby.SmartStandbyMode.LastBrew),
        new SmartStandby(TimeSpan.FromMinutes(30), SmartStandby.SmartStandbyMode.LastBrew),
        new SmartStandby(TimeSpan.FromHours(2), SmartStandby.SmartStandbyMode.LastBrew),
        new SmartStandby(TimeSpan.FromHours(2), SmartStandby.SmartStandbyMode.PowerOn),
    };

    [Theory]
    [MemberData(nameof(SmartStandbyTestData))]
    public async Task SetSmartStandbyAsyncTest(SmartStandby standby)
    {
        var mode =
            standby.Mode == SmartStandby.SmartStandbyMode.LastBrew ? "LastBrewing" : "PowerOn";
        var machineConnectionMock = new Mock<IMachineConnection>();
        var service = new Domain.Services.Machine(machineConnectionMock.Object);
        await service.SetSmartStandbyAsync(standby, CancellationToken.None);
        machineConnectionMock.Verify(
            m =>
                m.WriteValueAsync(
                    "SettingSmartStandby",
                    Is.Obj(
                        new
                        {
                            minutes = standby.Time.TotalMinutes,
                            mode,
                            enabled = true,
                        }
                    ),
                    Is.Ct
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task SetSmartStandbyOffAsyncTest()
    {
        var machineConnectionMock = new Mock<IMachineConnection>();
        var service = new Domain.Services.Machine(machineConnectionMock.Object);
        await service.SetSmartStandbyAsync(null, CancellationToken.None);
        machineConnectionMock.Verify(
            m =>
                m.WriteValueAsync(
                    "SettingSmartStandby",
                    Is.Obj(
                        new
                        {
                            minutes = 0,
                            mode = "LastBrewing",
                            enabled = false,
                        }
                    ),
                    Is.Ct
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task GetBoilersAsyncTest()
    {
        var machineConnectionMock = new Mock<IMachineConnection>();
        machineConnectionMock
            .Setup(m => m.ReadValueAsync(IMachineConnection.ReadSetting.Boilers, Is.Ct))
            .Returns(
                Task.FromResult(
                    "[{\"id\":\"CoffeeBoiler1\",\"isEnabled\":true,\"target\":94,\"current\":93},{\"id\":\"SteamBoiler\",\"isEnabled\":true,\"target\":101,\"current\":118}]"
                )
            );
        var service = new Domain.Services.Machine(machineConnectionMock.Object);
        var boilers = await service.GetBoilersAsync(CancellationToken.None);
        Assert.NotNull(boilers.CoffeeBoiler);
        Assert.NotNull(boilers.SteamBoiler);
        Assert.True(boilers.CoffeeBoiler.IsEnabled);
        Assert.True(boilers.SteamBoiler.IsEnabled);
        Assert.Equal(94, boilers.CoffeeBoiler.TargetTemperature);
        Assert.Equal(93, boilers.CoffeeBoiler.Temperature);
        Assert.Equal(2, boilers.SteamBoiler.TargetLevel);
        Assert.Equal(118, boilers.SteamBoiler.Temperature);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(60)]
    [InlineData(70)]
    [InlineData(80)]
    [InlineData(85)]
    [InlineData(90)]
    [InlineData(95)]
    public async Task SetBoilerTargetTemperatureAsyncTest(int temperature)
    {
        var machineConnectionMock = new Mock<IMachineConnection>();
        var service = new Domain.Services.Machine(machineConnectionMock.Object);
        await service.SetBoilerTargetTemperatureAsync(temperature, CancellationToken.None);
        machineConnectionMock.Verify(
            m =>
                m.WriteValueAsync(
                    "SettingBoilerTarget",
                    Is.Obj(new { identifier = "CoffeeBoiler1", value = temperature }),
                    Is.Ct
                ),
            Times.Once
        );
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task SetSteamLevelAsyncTest(int level)
    {
        var temperature = level switch
        {
            1 => 100,
            2 => 101,
            3 => 102,
            _ => 0,
        };
        var machineConnectionMock = new Mock<IMachineConnection>();
        var service = new Domain.Services.Machine(machineConnectionMock.Object);
        await service.SetSteamLevelAsync(level, CancellationToken.None);
        machineConnectionMock.Verify(
            m =>
                m.WriteValueAsync(
                    "SettingBoilerTarget",
                    Is.Obj(new { identifier = "SteamBoiler", value = temperature }),
                    Is.Ct
                ),
            Times.Once
        );
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task SetSteamBoilerEnabledAsyncTest(bool enabled)
    {
        var machineConnectionMock = new Mock<IMachineConnection>();
        var service = new Domain.Services.Machine(machineConnectionMock.Object);
        await service.SetSteamBoilerEnabledAsync(enabled, CancellationToken.None);
        machineConnectionMock.Verify(
            m =>
                m.WriteValueAsync(
                    "SettingBoilerEnabled",
                    Is.Obj(new { identifier = "SteamBoiler", value = enabled }),
                    Is.Ct
                ),
            Times.Once
        );
    }
}

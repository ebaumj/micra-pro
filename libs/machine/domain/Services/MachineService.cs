using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.Machine.DataDefinition;
using MicraPro.Machine.Domain.BluetoothAccess;
using MicraPro.Machine.Domain.DatabaseAccess;
using MicraPro.Machine.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MicraPro.Machine.Domain.Services;

public class MachineService(
    IBluetoothService bluetoothService,
    IServiceScopeFactory serviceScopeFactory,
    IMachineConnectionFactory machineConnectionFactory,
    IMachineFactory machineFactory,
    ILogger<MachineService> logger
) : IMachineService, IHostedService
{
    private static readonly TimeSpan DiscoverTime = TimeSpan.FromSeconds(30);
    private IMachineRepository Repository =>
        serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IMachineRepository>();
    private readonly BehaviorSubject<IMachineConnection?> _connection = new(null);
    public IObservable<IMachine?> MachineObservable =>
        _connection.DistinctUntilChanged().Select(c => c == null ? null : machineFactory.Create(c));
    public IMachine? Machine =>
        _connection.Value == null ? null : machineFactory.Create(_connection.Value);

    public async Task ConnectAsync(string id, CancellationToken ct)
    {
        var repo = Repository;
        var machine = await repo.GetCurrentMachineAsync(ct);
        if (machine != id)
            await repo.SetCurrentMachineAsync(id, ct);
        _connection.OnNext(await machineConnectionFactory.CreateAsync(id, ct));
    }

    public async Task DisconnectAsync(CancellationToken ct)
    {
        await (_connection.Value?.DisconnectAsync(ct) ?? Task.CompletedTask);
        _connection.OnNext(null);
        await Repository.RemoveCurrentMachineAsync(ct);
    }

    public IObservable<IMachineService.MachineScanResult> Scan(CancellationToken ct)
    {
        var subject = new ReplaySubject<IMachineService.MachineScanResult>();
        var subscriptions = new CompositeDisposable();
        subscriptions.Add(
            Observable
                .FromAsync(() => bluetoothService.DiscoverAsync(DiscoverTime, ct))
                .Subscribe(_ =>
                {
                    subscriptions.Add(
                        bluetoothService.DetectedDevices.Subscribe(d =>
                            subject.OnNext(
                                new IMachineService.MachineScanResult(d.Name, d.BluetoothId)
                            )
                        )
                    );
                    subscriptions.Add(
                        bluetoothService
                            .IsScanning.Where(s => !s)
                            .Subscribe(_ =>
                            {
                                subscriptions.Dispose();
                                subject.OnCompleted();
                            })
                    );
                })
        );
        ct.Register(() =>
        {
            subscriptions.Dispose();
            subject.OnCompleted();
        });
        return subject.AsObservable();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var machineId = await Repository.GetCurrentMachineAsync(cancellationToken);
        if (machineId != null)
            try
            {
                _connection.OnNext(
                    await machineConnectionFactory.CreateAsync(machineId, cancellationToken)
                );
                logger.LogInformation("Machine {id} Connected", machineId);
            }
            catch (Exception e)
            {
                logger.LogError("failed to connect machine: {e}", e);
            }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await (_connection.Value?.DisconnectAsync(cancellationToken) ?? Task.CompletedTask);
        }
        catch (Exception e)
        {
            logger.LogError("failed to disconnect machine: {e}", e);
        }
        _connection.OnNext(null);
    }
}

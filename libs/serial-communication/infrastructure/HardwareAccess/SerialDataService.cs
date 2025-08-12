using System.IO.Ports;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.SerialCommunication.Domain.HardwareAccess;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MicraPro.SerialCommunication.Infrastructure.HardwareAccess;

public class SerialDataService(
    IOptions<SerialCommunicationInfrastructureOptions> options,
    ILogger<SerialDataService> logger
) : ISerialDataService, IHostedService
{
    private static readonly TimeSpan ReadPeriod = TimeSpan.FromMilliseconds(5);
    private const int BufferSize = 100;
    private const byte StartByte = 2;
    private const byte EndByte = 3;

    private IDisposable _subscription = Disposable.Empty;
    private SerialPort? _serialPort;
    private readonly Subject<byte[]> _receiveSubject = new();

    public async Task SendAsync(byte[] data, CancellationToken ct)
    {
        var buffer = new char[100];
        var len = Convert.ToBase64CharArray(data, 0, data.Length, buffer, 0);
        byte[] encodedData = [StartByte, .. buffer.Take(len).Select(c => (byte)c), EndByte];
        if (_serialPort == null)
            return;
        await _serialPort.BaseStream.WriteAsync(encodedData, ct);
    }

    public IObservable<byte[]> Received => _receiveSubject;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _serialPort = new SerialPort(
            options.Value.SerialPort,
            options.Value.BaudRate,
            Parity.None,
            8,
            StopBits.One
        );
        _serialPort.ReadTimeout = 200;
        _serialPort.WriteTimeout = 200;
        try
        {
            _serialPort.Open();
        }
        catch (Exception e)
        {
            logger.LogError("Failed to open Serial Port: {e}", e);
            _serialPort = null;
            return Task.CompletedTask;
        }
        _serialPort.DiscardInBuffer();
        var byteStream = Observable
            .Interval(ReadPeriod)
            .Select(_ =>
            {
                var buffer = new byte[BufferSize];
                var bytes =
                    _serialPort.BytesToRead > 0 ? _serialPort.Read(buffer, 0, BufferSize) : 0;
                return buffer.Take(bytes).ToArray();
            })
            .SelectMany(b => b);
        _subscription = Observable
            .Create<List<byte>>(observer =>
            {
                var buffer = new List<byte>();
                var isBuffering = false;
                return byteStream.Subscribe(
                    item =>
                    {
                        if (!isBuffering && item != StartByte)
                            return;
                        isBuffering = true;
                        buffer.Add(item);
                        if (item != EndByte)
                            return;
                        isBuffering = false;
                        observer.OnNext(buffer);
                        buffer = [];
                    },
                    observer.OnError,
                    () =>
                    {
                        if (isBuffering)
                            observer.OnNext(buffer);
                        observer.OnCompleted();
                    }
                );
            })
            .Select(buffer => buffer.Skip(1).SkipLast(1).ToArray())
            .Subscribe(data =>
            {
                _receiveSubject.OnNext(
                    Convert.FromBase64CharArray(data.Select(b => (char)b).ToArray(), 0, data.Length)
                );
            });
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _serialPort?.Close();
        _subscription.Dispose();
        return Task.CompletedTask;
    }
}

using System.Reactive.Linq;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess.Dummy;

// Dirty Code, for testing purposes only
internal class DummyConnection : IBleDeviceConnection
{
    private bool _isTared;

    private class DummyBleService(DummyConnection connection) : IBleService
    {
        private class DummyCharacteristic(DummyConnection connection) : IBleCharacteristic
        {
            private readonly Random _rand = new();
            private long _firstIteration;

            public Task SendCommandAsync(byte[] data, CancellationToken ct)
            {
                connection._isTared = true;
                return Task.CompletedTask;
            }

            private static byte[] FromData(double flow, double weight)
            {
                var flowInt = Convert.ToInt32(Math.Round(flow * 100));
                var weightInt = Convert.ToInt32(Math.Round(weight * 100));
                var data = new byte[20];
                data[0] = 0x03;
                data[1] = 0x0B;
                data[9] = (byte)(weightInt & 0xFF);
                data[8] = (byte)((weightInt >> 8) & 0xFF);
                data[7] = (byte)((weightInt >> 16) & 0xFF);
                data[11] = (byte)((flowInt >> 8) & 0xFF);
                data[12] = (byte)(flowInt & 0xFF);
                data[19] = (byte)data.Take(19).Aggregate(0, (acc, d) => acc ^ d);
                return data;
            }

            public Task<IObservable<byte[]>> GetValueObservableAsync(CancellationToken ct) =>
                Task.FromResult(
                    Observable
                        .Interval(TimeSpan.FromMilliseconds(100))
                        .Select(iteration =>
                        {
                            double flow = 0;
                            double weight = 0;
                            if (connection._isTared && iteration > _firstIteration)
                            {
                                weight = (iteration - _firstIteration) * 0.2;
                                flow = weight < 50 ? 1 + _rand.NextDouble() * 0.2 : 0;
                            }
                            else if (!connection._isTared)
                                _firstIteration = iteration + 2;
                            return FromData(flow, weight);
                        })
                );
        }

        public Task<IBleCharacteristic> GetCharacteristicAsync(
            string characteristicId,
            CancellationToken ct
        ) => Task.FromResult<IBleCharacteristic>(new DummyCharacteristic(connection));
    }

    public Task<IBleService> GetServiceAsync(string serviceId, CancellationToken ct) =>
        Task.FromResult<IBleService>(new DummyBleService(this));

    public Task Disconnect(CancellationToken ct) => Task.CompletedTask;
}

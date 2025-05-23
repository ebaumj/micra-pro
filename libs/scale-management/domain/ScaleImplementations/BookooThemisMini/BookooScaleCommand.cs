namespace MicraPro.ScaleManagement.Domain.ScaleImplementations.BookooThemisMini;

public abstract record BookooScaleCommand
{
    private static readonly byte[] Header = [0x03, 0x0A];
    protected abstract byte[] Data { get; }

    public byte[] Serialized => Header.Concat(Data).Concat([Checksum]).ToArray();

    private byte Checksum =>
        (byte)(Header.Concat(Data).Aggregate(0, (acc, value) => acc ^ value) & 0xFF);

    public record Tare : BookooScaleCommand
    {
        protected override byte[] Data => [0x01, 0x00, 0x00];
    }
}

namespace HeavenMaria.Domain.Interfaces
{
    public interface ICompressionService
    {
        byte[] CompressToByteArray(string text);
        string DecompressToString(byte[] compressed);
        byte[] DecompressToByteArray(byte[] compressed);
    }
}

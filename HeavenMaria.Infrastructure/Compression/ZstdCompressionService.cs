using HeavenMaria.Domain.Interfaces;
using System.Text;
using ZstdSharp;

namespace HeavenMaria.Infrastructure.Compression
{
    public class ZstdCompressionService : ICompressionService
    {
        // デフォルトの圧縮レベル (3) を使用
        private const int CompressionLevel = 3;

        public byte[] CompressToByteArray(string text) => Compress(text);
        public byte[] DecompressToByteArray(byte[] compressed) => Decompress(compressed);
        public string DecompressToString(byte[] compressed) => Encoding.UTF8.GetString(Decompress(compressed));

        private byte[] Compress(string text)
        {
            var source = Encoding.UTF8.GetBytes(text);
            return CompressFromByteArray(source);
        }

        private byte[] CompressFromByteArray(byte[] source)
        {
            // 圧縮レベルを指定して Compressor のインスタンスを作成
            using (var compressor = new Compressor(CompressionLevel))
            {
                // Compress() メソッドで圧縮を実行
                return compressor.Wrap(source).ToArray();
            }
        }

        private byte[] Decompress(byte[] compressedData)
        {
            // Decompressor のインスタンスを作成
            using (var decompressor = new Decompressor())
            {
                // Unwrap() メソッドで解凍を実行
                return decompressor.Unwrap(compressedData).ToArray();
            }
        }
    }
}

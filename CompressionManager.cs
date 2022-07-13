using System;
using System.Linq;
using K4os.Compression.LZ4;

namespace YAHGA_Server
{
    public static class CompressionManager
    {
        public static byte[] Compress(byte[] source)
        {
            var target = new byte[LZ4Codec.MaximumOutputSize(source.Length)];
            var encodedLength = LZ4Codec.Encode(
                source, 0, source.Length,
                target, 0, target.Length,
                LZ4Level.L12_MAX);

            return target.Take(encodedLength).ToArray();
        }
        
        public static byte[] Decompress(byte[] source)
        {
            var target = new byte[source.Length * 255];
            var decoded = LZ4Codec.Decode(
                source, 0, source.Length,
                target, 0, target.Length);

            return target.Take(decoded).ToArray();
        }

        public static string CompressAndBase64Encode(byte[] source)
        {
            var bytes = Compress(source);
            return Convert.ToBase64String(bytes);
        }

        public static byte[] DecompressAndBase64Decode(string source)
        {
            var bytes = Convert.FromBase64String(source);
            return Decompress(bytes);
        }
    }
}
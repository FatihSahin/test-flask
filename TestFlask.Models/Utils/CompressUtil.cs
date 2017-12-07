using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestFlask.Models.Utils
{
    public static class CompressUtil
    {
        /// <summary>
        /// Compresses a string and returns a deflate compressed, Base64 encoded string.
        /// </summary>
        /// <param name="uncompressedString">String to compress</param>
        public static string CompressString(string uncompressedString)
        {
            var compressedStream = new MemoryStream();
            var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString));

            using (var compressorStream = new DeflateStream(compressedStream, CompressionMode.Compress, true))
            {
                uncompressedStream.CopyTo(compressorStream);
            }

            return Convert.ToBase64String(compressedStream.ToArray());
        }

        /// <summary>
        /// Decompresses a deflate compressed, Base64 encoded string and returns an uncompressed string.
        /// </summary>
        /// <param name="compressedString">String to decompress.</param>
        public static string DecompressString(string compressedString)
        {
            var decompressedStream = new MemoryStream();
            var compressedStream = new MemoryStream(Convert.FromBase64String(compressedString));

            using (var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                decompressorStream.CopyTo(decompressedStream);
            }

            return Encoding.UTF8.GetString(decompressedStream.ToArray());
        }
    }
}

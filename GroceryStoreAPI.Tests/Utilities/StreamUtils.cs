using System.IO;
using System.Text;

namespace GroceryStoreAPI.Tests.Utilities
{
    public static class StreamUtils
    {
        public static StreamReader MakeStreamReader(string data) =>
            new(new MemoryStream(Encoding.UTF8.GetBytes(data)));

        public static StreamWriter MakeStreamWriter() => new(new MemoryStream());

        public static string ReadStream(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}

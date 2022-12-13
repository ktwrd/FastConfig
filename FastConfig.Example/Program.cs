using System.Diagnostics;
using System.Reflection;

namespace FastConfig.Example
{
    public class Config
    {
        [Group("General")]
        public bool Enable { get; set; }
        public AuthConfig Authentication { get; set; } = new();
    }
    [Inner]
    [Group("Authentication")]
    public class AuthConfig
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public bool Remember { get; set; }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var fastConfig = new FastConfigSource<Config>(@"example.ini");
            var parsed = fastConfig.Parse();
            Debugger.Break();
        }
    }
}
using FastConfig;

namespace FastConfigExample
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
            var fastConfig = new FastConfigSource<Config>(@"example.ini");
            var parsed = fastConfig.Parse();
            var dict = fastConfig.ToDictionary(parsed);

            PrintContent(fastConfig, parsed, dict);
        }
        static void PrintContent(FastConfigSource<Config> fastConfig, Config parsed, Dictionary<string, Dictionary<string, object>> dict)
        {
            var serializer = new System.Text.Json.JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                WriteIndented = true,
                IncludeFields = true
            };
            Console.WriteLine($"================================ Text Input");
            Console.WriteLine(File.ReadAllText("example.ini"));
            Console.WriteLine($"================================ Parsed");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(parsed, serializer));
            Console.WriteLine($"================================ To Dictionary");
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(dict, serializer));
            Console.WriteLine($"================================ Output Content");
            Console.WriteLine(string.Join("\n", fastConfig.ToFileLines(parsed)));
            Console.WriteLine("\n\n\n\n\n\n\n\n");
        }
    }
}
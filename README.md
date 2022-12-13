## Usage
See [FastConfigExample/Program.cs](FastConfigExample/Program.cs) for a full example
```csharp
using FastConfig;

namespace FastConfigExample
{
    public class Config
    {
        // Indicate that this field belongs in the "General" group
        [Group("General")]
        public bool Enable { get; set; }
        public AuthConfig Authentication { get; set; }
    }
    // Define that this is a nested item and should be searched
    [FastSerialize]
    // Default all fields and properties to the "Authentication" group
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
            // Load from "example.ini"
            FastConfigSource<Config> fastConfig = new FastConfigSource<Config>(@"example.ini");

            // Deserialize file content to instance of Config.
            Config parsed = fastConfig
                .Parse();

            // Serialize instance of Config to a dictionary.
            Dictionary<string, Dictionary<string, object>> dict = fastConfig
                .ToDictionary(parsed);

            // Create Ini file content from the deserialized
            // class, or any class that is the same type.
            string[] fileLines = fastConfig.ToFileLines(parsed);

            // Save config to desierd location
            fastConfig.Save(parsed, $"example-{DateTimeOffset.Now.ToUnixSeconds()}.ini").Wait();
        }
    }
}
```
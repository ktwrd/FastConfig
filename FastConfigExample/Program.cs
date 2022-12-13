using FastConfig;
using System;
using System.IO;
using System.Collections.Generic;

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
    [ConfigSerialize]
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

            PrintContent(parsed, dict, fileLines);
        }
        static void PrintContent(Config parsed, Dictionary<string, Dictionary<string, object>> dict, string[] fileLines)
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
            Console.WriteLine(string.Join("\n", fileLines));
        }
    }
}
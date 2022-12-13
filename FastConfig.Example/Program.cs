﻿using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

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
            var fastConfig = new FastConfigSource<Config>(@"example.ini");
            var parsed = fastConfig.Parse();
            var dict = fastConfig.ToDictionary(parsed);

            PrintContent(parsed, dict);
        }
        static void PrintContent(Config parsed, Dictionary<string, Dictionary<string, object>> dict)
        {
            var serializer = new JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                WriteIndented = true,
                IncludeFields = true
            };
            Console.WriteLine($"================================ Text Input");
            Console.WriteLine(File.ReadAllText("example.ini"));
            Console.WriteLine($"================================ Parsed");
            Console.WriteLine(JsonSerializer.Serialize(parsed, serializer));
            Console.WriteLine($"================================ To Dictionary");
            Console.WriteLine(JsonSerializer.Serialize(dict, serializer));
            Console.WriteLine("\n\n\n\n\n\n\n\n");
        }
    }
}
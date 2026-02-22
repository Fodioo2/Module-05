using System;
using System.Collections.Generic;
using System.IO;

public sealed class ConfigurationManager
{
    private static ConfigurationManager instance;
    private static readonly object lockObj = new object();

    private Dictionary<string, string> settings = new Dictionary<string, string>();

    private ConfigurationManager() { }

    public static ConfigurationManager GetInstance()
    {
        lock (lockObj)
        {
            if (instance == null)
                instance = new ConfigurationManager();

            return instance;
        }
    }

    public void Set(string key, string value)
    {
        settings[key] = value;
    }

    public string Get(string key)
    {
        if (settings.ContainsKey(key))
            return settings[key];

        throw new Exception("Setting not found");
    }

    public void SaveToFile(string path)
    {
        using var writer = new StreamWriter(path);
        foreach (var pair in settings)
            writer.WriteLine($"{pair.Key}={pair.Value}");
    }

    public void LoadFromFile(string path)
    {
        if (!File.Exists(path)) return;

        foreach (var line in File.ReadAllLines(path))
        {
            var parts = line.Split('=');
            if (parts.Length == 2)
                settings[parts[0]] = parts[1];
        }
    }
}
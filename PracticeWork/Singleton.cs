using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public enum LogLevel
{
    INFO = 0,
    WARNING = 1,
    ERROR = 2
}

public class LoggerConfig
{
    public string LogFilePath { get; set; } = "app.log";
    public LogLevel CurrentLevel { get; set; } = LogLevel.INFO;
    public bool LogToConsole { get; set; } = true;

    public long MaxFileSizeBytes { get; set; } = 50_000; 
}

public sealed class Logger
{
    private static readonly Lazy<Logger> lazy =
        new Lazy<Logger>(() => new Logger(), LazyThreadSafetyMode.ExecutionAndPublication);

    public static Logger GetInstance() => lazy.Value;

    private readonly object fileLock = new object();
    private LoggerConfig config = new LoggerConfig();
    private string configPath = "loggerconfig.json";

    private Logger()
    {
        LoadConfig(configPath);
    }

    public void LoadConfig(string path)
    {
        configPath = path;

        if (!File.Exists(path))
        {

            SaveConfig(path);
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            var loaded = JsonSerializer.Deserialize<LoggerConfig>(json);
            if (loaded != null) config = loaded;
        }
        catch
        {

        }
    }

    public void SaveConfig(string path)
    {
        string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    public void SetLogLevel(LogLevel level)
    {
        config.CurrentLevel = level;
        SaveConfig(configPath);
    }

    public void Log(string message, LogLevel level)
    {
        if (level < config.CurrentLevel) return;

        string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {message}";

        lock (fileLock)
        {
            RotateIfNeeded();
            File.AppendAllText(config.LogFilePath, line + Environment.NewLine);
        }

        if (config.LogToConsole)
        {
            Console.WriteLine(line);
        }
    }

    private void RotateIfNeeded()
    {
        try
        {
            if (!File.Exists(config.LogFilePath)) return;

            var info = new FileInfo(config.LogFilePath);
            if (info.Length < config.MaxFileSizeBytes) return;

            string dir = Path.GetDirectoryName(config.LogFilePath) ?? "";
            string name = Path.GetFileNameWithoutExtension(config.LogFilePath);
            string ext = Path.GetExtension(config.LogFilePath);
            string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            string rotated = Path.Combine(dir, $"{name}_{stamp}{ext}");
            File.Move(config.LogFilePath, rotated);
        }
        catch
        {
        }
    }
}

public class LogReader
{
    private readonly string logPath;

    public LogReader(string logFilePath)
    {
        logPath = logFilePath;
    }

    public List<string> ReadAll()
    {
        if (!File.Exists(logPath)) return new List<string>();
        return new List<string>(File.ReadAllLines(logPath));
    }

    public List<string> FilterByLevel(LogLevel minLevel)
    {
        var lines = ReadAll();
        var result = new List<string>();

        foreach (var line in lines)
        {
            if (line.Contains("[ERROR]") && minLevel <= LogLevel.ERROR) result.Add(line);
            else if (line.Contains("[WARNING]") && minLevel <= LogLevel.WARNING) result.Add(line);
            else if (line.Contains("[INFO]") && minLevel <= LogLevel.INFO) result.Add(line);
        }

        return result;
    }
}

public class Program_LoggerPractice
{
    public static void Main()
    {
        var logger = Logger.GetInstance();

        logger.SetLogLevel(LogLevel.INFO);

        Console.WriteLine("== Multithread logging test ==");
        var tasks = new List<Task>();

        for (int t = 1; t <= 5; t++)
        {
            int threadId = t;
            tasks.Add(Task.Run(() =>
            {
                for (int i = 1; i <= 20; i++)
                {
                    LogLevel level = (i % 7 == 0) ? LogLevel.ERROR :
                                     (i % 3 == 0) ? LogLevel.WARNING :
                                                    LogLevel.INFO;

                    logger.Log($"Thread {threadId}: message {i}", level);
                    Thread.Sleep(10);
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());

        Console.WriteLine();
        Console.WriteLine("== Read only ERROR logs ==");
        var reader = new LogReader("app.log");
        var errors = reader.FilterByLevel(LogLevel.ERROR);

        foreach (var line in errors)
            Console.WriteLine(line);

        Console.WriteLine("== Done ==");
    }
}
using System.IO;
using System.Text.Json;

namespace EchoForge;

public static class AppSettings
{
    private class SettingsData
    {
        public bool EnableTransitions { get; set; } = true;
    }

    private static readonly string FilePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "settings.json");

    public static bool EnableTransitions { get; set; } = true;

    public static void Load()
    {
        if (!File.Exists(FilePath)) return;
        try
        {
            var json = File.ReadAllText(FilePath);
            var data = JsonSerializer.Deserialize<SettingsData>(json);
            if (data != null)
            {
                EnableTransitions = data.EnableTransitions;
            }
        }
        catch
        {
            // ignore corrupt settings
        }
    }

    public static void Save()
    {
        var data = new SettingsData { EnableTransitions = EnableTransitions };
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        try
        {
            File.WriteAllText(FilePath, json);
        }
        catch
        {
            // ignore write errors
        }
    }
}

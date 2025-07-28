using System.IO;
using System.Text.Json;
using Microsoft.Win32;

namespace EchoForge.Services;

public class ProjectData
{
    public string SelectedView { get; set; } = "Input";
    public bool EnableTransitions { get; set; }
    public string? Manuscript { get; set; }
}

public static class ProjectService
{
    public static void Save(ProjectData data)
    {
        var dialog = new SaveFileDialog
        {
            Filter = "EchoForge Project (*.echo)|*.echo",
            DefaultExt = ".echo"
        };
        if (dialog.ShowDialog() == true)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(dialog.FileName, json);
        }
    }

    public static ProjectData? Load()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "EchoForge Project (*.echo)|*.echo",
            DefaultExt = ".echo"
        };
        if (dialog.ShowDialog() == true)
        {
            var json = File.ReadAllText(dialog.FileName);
            return JsonSerializer.Deserialize<ProjectData>(json);
        }
        return null;
    }
}

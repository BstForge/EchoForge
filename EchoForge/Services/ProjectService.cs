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
    public static string? Save(ProjectData data)
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
            return dialog.FileName;
        }
        return null;
    }

    public static (ProjectData? Data, string? Path) Load()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "EchoForge Project (*.echo)|*.echo",
            DefaultExt = ".echo"
        };
        if (dialog.ShowDialog() == true)
        {
            var json = File.ReadAllText(dialog.FileName);
            var data = JsonSerializer.Deserialize<ProjectData>(json);
            return (data, dialog.FileName);
        }
        return (null, null);
    }
}

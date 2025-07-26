using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
public static class SettingsManager
{
    private static readonly string settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

    public static void Save(SettingsModel settings)
    {
        try
        {
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(settingsFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving settings: " + ex.Message);
        }
    }

    public static SettingsModel Load()
    {
        if (!File.Exists(settingsFilePath))
            return GetDefaultSettings();

        try
        {
            string json = File.ReadAllText(settingsFilePath);
            return JsonConvert.DeserializeObject<SettingsModel>(json) ?? GetDefaultSettings();
        }
        catch (Exception)
        {
            return GetDefaultSettings();
        }
    }

    private static SettingsModel GetDefaultSettings()
    {
        return new SettingsModel
        {
            CrosshairColor = Color.Red.Name,
            CrosshairThickness = 2,
            CrosshairSize = 20,
            SelectedShape = CrosshairShape.Cruz,
            IsCrosshairVisible = true
        };
    }
}
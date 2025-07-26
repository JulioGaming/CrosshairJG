using System;
using System.IO;
using Newtonsoft.Json;
using System.Drawing;

public class SettingsModel
{
    public bool IsCrosshairVisible { get; set; } = true;
    public int CrosshairSize { get; set; } = 20;
    public string CrosshairColor { get; set; } = Color.Red.Name;
    public int CrosshairThickness { get; set; } = 2;
    public float CrosshairScale { get; set; } = 1.0f;
    public Point CrosshairPosition { get; set; } = Point.Empty;
    public CrosshairShape SelectedShape { get; set; } = CrosshairShape.Cruz;

    [JsonIgnore]
    public Color CrosshairColorParsed => Color.FromName(CrosshairColor);
}
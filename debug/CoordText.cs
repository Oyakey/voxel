using System.Globalization;
using Godot;

namespace Voxel.Debug;

public partial class CoordText : Label
{
    public override void _Ready()
    {
        Text = string.Empty;
    }
    public void _process(float _)
    {
        var playerPosition = Main.Player.Position;

        // Shorten the number to n decimal places and add 00 after the decimal point.
        // We use InvariantCulture to ensure that the decimal separator is always a dot (ToString is locale-dependent).
        var formattedX = playerPosition.X.ToString("0.000", CultureInfo.InvariantCulture);
        var formattedY = playerPosition.Y.ToString("0.00000", CultureInfo.InvariantCulture);
        var formattedZ = playerPosition.Z.ToString("0.000", CultureInfo.InvariantCulture);

        Text = $"XYZ: {formattedX} / {formattedY} / {formattedZ}";
    }
}

using Godot;

public partial class Main : Node3D
{
    private static readonly PerlinNoise noiseGenerator = new();

    public static float WorldGenerator(Vector3 position)
    {
        int scale = 8;
        return noiseGenerator.Noise(position.X * scale, position.Z * scale) * 32;
    }
}

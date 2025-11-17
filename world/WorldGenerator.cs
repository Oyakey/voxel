namespace Voxel.World;

public class WorldGenerator()
{
    private Node3D _chunkParent;
    private readonly PerlinNoise noiseGenerator = new();

    public Init(Node3D chunkParent)
    {
        _chunkParent = chunkParent;
    }

    // TODO: Replace this by an octave noise generator.
    public static float Noise(Vector3 position)
    {
        int scale = 4;
        return noiseGenerator.Noise(position.X / scale, position.Z / scale);
    }

}

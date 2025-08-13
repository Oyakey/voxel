using Godot;
using Voxel.Chunk;

namespace Voxel;

public partial class Main : Node3D
{
    private long _baseSeed = 123456789;
    private long _worldSeed;
    private long _chunkSeed = 0;

    private static ChunkGenerator _chunkGenerator;

    private static readonly PerlinNoise noiseGenerator = new();

    public static ChunkGenerator ChunkGenerator => _chunkGenerator;

    public static ChunkCoords PlayerCurrentChunk { get; set; }

    public static float WorldGenerator(Vector3 position)
    {
        int scale = 4;
        return noiseGenerator.Noise(position.X / scale, position.Z / scale);
    }

    public void GenerateWorldSeed(float x, float y)
    {
        _worldSeed = _baseSeed;
        _worldSeed *= _worldSeed * 6364136223846793005L + 1442695040888963407L;
        _worldSeed *= _worldSeed * 6364136223846793005L + 1442695040888963407L;
        _worldSeed = _baseSeed;
        _worldSeed *= _worldSeed * 6364136223846793005L + 1442695040888963407L;
        _worldSeed *= _worldSeed * 6364136223846793005L + 1442695040888963407L;
    }

    private void _ready()
    {
        _chunkGenerator = new ChunkGenerator(GetNode<Node3D>("Chunks"));
        ChunkGenerator.RenderChunk(new ChunkCoords(0, 0));
    }
}

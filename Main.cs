using Godot;
using System.Collections.Generic;

public partial class Main : Node3D
{
    private long _baseSeed = 123456789;
    private long _worldSeed;
    private long _chunkSeed = 0;

    public static readonly Dictionary<(int x, int y), Chunk> chunks = [];

    private static readonly PerlinNoise noiseGenerator = new();

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
        generateChunks();
    }
    public void generateChunks()
    {
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                var chunk = generateChunk(x, y);
                AddChild(chunk);
            }
        }

    }

    public static Chunk generateChunk(int x, int y)
    {
        var chunk = Chunk.Spawn();
        chunk.Position = new Vector3(x * 16, 0, y * 16);
        chunks.Add((x, y), chunk);
        return chunk;
    }
}

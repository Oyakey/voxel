using Godot;
using System.Collections.Generic;

public partial class Chunk : Node3D
{
    private readonly Dictionary<(int x, int y, int z), BlockData> _blocks = [];
    private const int sideLength = 16;
    private const int minHeight = -32;
    private const int maxHeight = 32;
    private long _worldSeed = 123456789;
    private long _chunkSeed;

    private static readonly PackedScene chunkPrefab = ResourceLoader
      .Load<PackedScene>("res://chunk/chunk.tscn");
    private static readonly PackedScene blockPrefab = ResourceLoader
      .Load<PackedScene>("res://block/block.tscn");

    public static Chunk Spawn()
    {
        var chunk = chunkPrefab.Instantiate<Chunk>();
        return chunk;
    }

    public void RenderBlock(BlockData blockData, int renderMode = 0)
    {
        if (renderMode == 0)
            return;
        if (blockData.Type == BlockType.Air)
            return;
        var block = blockPrefab.Instantiate<Block>();
        block.Position = blockData.Position;
        AddChild(block);
        block.SetBlockData(blockData);
        block.RenderFaces(renderMode);
    }

    private void _ready()
    {
        Dictionary<(int x, int y, int z), BlockData> blocks = [];
        GenerateChunkSeed(Position.X, Position.Y);
        generateBlocks();
        renderBlocks();

        // Logic for destrying and recreating the chunk will be done later.
    }

    private void generateBlocks()
    {
        for (var x = 0; x < sideLength; x++)
        {
            for (var z = 0; z < sideLength; z++)
            {
                for (var y = minHeight; y < maxHeight; y++)
                    _blocks.Add((x, y, z), GetBlockData(new Vector3(x, y, z)));
            }
        }
    }

    private void renderBlocks()
    {
        // We need to check what blocks should be rendered
        // They are :
        // 1. Blocks that are in the chunk (in _blocks)
        // 2. Blocks that are NOT surrounded by blocks on all sides
        // 3. Theses blocks should have only their visible faces rendered
        // We also need to make this operation asynchronous so that it won't freeze the game

        // TODO: The border between chunks should also be checked to avoid rendering hidden blocks.
        var defaultBlock = new BlockData(new Vector3(0, 0, 0), BlockType.Stone);

        for (var x = 0; x < sideLength; x++)
        {
            for (var z = 0; z < sideLength; z++)
            {
                for (var y = minHeight; y < maxHeight; y++)
                {
                    int renderMode = 0;

                    _blocks.TryGetValue((x, y, z), out var block);

                    if (block == null)
                    {
                        continue;
                    }

                    BlockData eastBlock = (x == sideLength - 1) ? defaultBlock : GetBlock(x + 1, y, z);
                    BlockData westBlock = (x == 0) ? defaultBlock : GetBlock(x - 1, y, z);
                    BlockData upBlock = GetBlock(x, y + 1, z);
                    BlockData downBlock = GetBlock(x, y - 1, z);
                    BlockData southBlock = (z == sideLength - 1) ? defaultBlock : GetBlock(x, y, z + 1);
                    BlockData northBlock = (z == 0) ? defaultBlock : GetBlock(x, y, z - 1);

                    if (eastBlock == null || eastBlock.Type == BlockType.Air)
                        renderMode += (int)Block.RenderMode.East;
                    if (westBlock == null || westBlock.Type == BlockType.Air)
                        renderMode += (int)Block.RenderMode.West;
                    if (upBlock == null || upBlock.Type == BlockType.Air)
                        renderMode += (int)Block.RenderMode.Up;
                    if (downBlock == null || downBlock.Type == BlockType.Air)
                        renderMode += 0;
                    // renderMode += (int)Block.RenderMode.Down;
                    if (southBlock == null || southBlock.Type == BlockType.Air)
                        renderMode += (int)Block.RenderMode.South;
                    if (northBlock == null || northBlock.Type == BlockType.Air)
                        renderMode += (int)Block.RenderMode.North;

                    RenderBlock(block, renderMode);
                }
            }
        }
    }

    public BlockData GetBlockData(Vector3 position)
    {
        var type = BlockType.Air;

        float height = WorldGenerator(position) * 8;

        if (position.Y < height)
            type = BlockType.Stone;

        return new BlockData(position, type);
    }

    public void GenerateChunkSeed(float x, float y)
    {
        _chunkSeed = _worldSeed;
        _chunkSeed *= _chunkSeed * 6364136223846793005L + 1442695040888963407L;
        _chunkSeed += (long)x;
        _chunkSeed *= _chunkSeed * 6364136223846793005L + 1442695040888963407L;
        _chunkSeed += (long)y;
        _chunkSeed *= _chunkSeed * 6364136223846793005L + 1442695040888963407L;
        _chunkSeed += (long)x;
        _chunkSeed *= _chunkSeed * 6364136223846793005L + 1442695040888963407L;
        _chunkSeed += (long)y;
    }

    private readonly PerlinNoise noiseGenerator = new();

    public float WorldGenerator(Vector3 position)
    {
        int scale = 4;
        return noiseGenerator.Noise(position.X / scale, position.Z / scale);
    }

    public BlockData GetBlock(int x, int y, int z)
    {
        if (x >= 0 && x < sideLength && y >= 0 && y < sideLength && z >= 0 && z < sideLength)
        {
            _blocks.TryGetValue((x, y, z), out var block);
            return block;
        }

        return null;

        int blockWorldCoordX = (int)Position.X + x;
        int blockWorldCoordY = (int)Position.Y + y;

        return TryGetBlock(blockWorldCoordX, blockWorldCoordY, z);
    }

    public static BlockData TryGetBlock(int x, int y, int z)
    {
        Main.chunks.TryGetValue((x - x % sideLength, y - y % sideLength), out var chunk);
        if (chunk == null)
            return null;
        return chunk.GetBlock(x % sideLength, y % sideLength, z);
    }

}

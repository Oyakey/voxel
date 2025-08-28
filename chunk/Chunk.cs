using Godot;

namespace Voxel.Chunk;

public partial class Chunk : Node3D
{
    public const int SIDE_LENGTH = 16;
    public const int MIN_HEIGHT = -32;
    public const int MAX_HEIGHT = 32;

    private long _worldSeed = 123456789;
    private long _chunkSeed;

    private ChunkData _chunkData;

    private static readonly PackedScene chunkPrefab = ResourceLoader
      .Load<PackedScene>("res://chunk/chunk.tscn");
    private static readonly PackedScene blockPrefab = ResourceLoader
      .Load<PackedScene>("res://block/block.tscn");

    public static Chunk Spawn(ChunkData chunkData)
    {
        var chunk = chunkPrefab.Instantiate<Chunk>();
        chunk._chunkData = chunkData;
        var x = chunkData.Coords.X;
        var y = chunkData.Coords.Y;
        chunk.Position = new Vector3(x * SIDE_LENGTH, 0, y * SIDE_LENGTH);
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

    public void _ready()
    {
        GenerateChunkSeed(Position.X, Position.Y);
        renderBlocks();

        // Logic for destrying and recreating the chunk will be done later.
    }

    private void renderBlocks()
    {
        GD.Print("Rendering chunk");
        // We need to check what blocks should be rendered
        // They are :
        // 1. Blocks that are in the chunk (in _blocks)
        // 2. Blocks that are NOT surrounded by blocks on all sides
        // 3. Theses blocks should have only their visible faces rendered
        // We also need to make this operation asynchronous so that it won't freeze the game

        // TODO: The border between chunks should also be checked to avoid rendering hidden blocks.
        var defaultBlock = new BlockData(new Vector3(0, 0, 0), BlockType.Stone);

        for (var x = 0; x < SIDE_LENGTH; x++)
        {
            for (var z = 0; z < SIDE_LENGTH; z++)
            {
                for (var y = MIN_HEIGHT; y < MAX_HEIGHT; y++)
                {
                    int renderMode = 0;

                    var block = _chunkData.GetBlock(new BlockCoords(x, y, z));

                    if (block == null)
                    {
                        continue;
                    }

                    BlockData eastBlock = _chunkData.GetBlock(new BlockCoords(x + 1, y, z));
                    BlockData westBlock = _chunkData.GetBlock(new BlockCoords(x - 1, y, z));
                    BlockData upBlock = _chunkData.GetBlock(new BlockCoords(x, y + 1, z));
                    BlockData downBlock = _chunkData.GetBlock(new BlockCoords(x, y - 1, z));
                    BlockData southBlock = _chunkData.GetBlock(new BlockCoords(x, y, z + 1));
                    BlockData northBlock = _chunkData.GetBlock(new BlockCoords(x, y, z - 1));

                    if (!IsBlockOpaque(eastBlock))
                        renderMode += (int)Block.RenderMode.East;
                    if (!IsBlockOpaque(westBlock))
                        renderMode += (int)Block.RenderMode.West;
                    if (!IsBlockOpaque(upBlock))
                        renderMode += (int)Block.RenderMode.Up;
                    if (!IsBlockOpaque(downBlock))
                        renderMode += (int)Block.RenderMode.Down;
                    if (!IsBlockOpaque(southBlock))
                        renderMode += (int)Block.RenderMode.South;
                    if (!IsBlockOpaque(northBlock))
                        renderMode += (int)Block.RenderMode.North;

                    RenderBlock(block, renderMode);
                }
            }
        }
    }

    private static bool IsBlockOpaque(BlockData blockData)
    {
        // Hacky workaround to avoid rendering blocks at the bottom of the chunk.
        // Remove when occlusion culling is implemented.
        if (blockData == null)
            return true;

        return blockData != null && blockData.Type != BlockType.Air || blockData.Position.Y > MIN_HEIGHT;
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
}

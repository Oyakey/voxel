using System.Collections.Generic;
using Godot;
using Voxel.Utils;

namespace Voxel.Chunk;

public class ChunkData
{
    public ChunkCoords Coords { get; private set; }

    private readonly Dictionary<BlockCoords, BlockData> _blocks = [];
    private readonly PerlinNoise noiseGenerator = new();


    public ChunkData(ChunkCoords coords)
    {
        Coords = coords;
        GenerateBlocks();
    }

    public BlockData GetBlock(BlockCoords blockCoords)
    {
        var blockWorldCoords = LocalToWorld(blockCoords);

        var chunkCoords = new ChunkCoords(
            Mathf.FloorToInt(blockWorldCoords.X / Chunk.SIDE_LENGTH),
            Mathf.FloorToInt(blockWorldCoords.Z / Chunk.SIDE_LENGTH)
        );
        var chunk = Main.ChunkGenerator.GetChunk(chunkCoords);
        if (chunk == null)
            return null;

        var localBlockCoords = new BlockCoords(
            Math.Mod(blockCoords.X, Chunk.SIDE_LENGTH),
            blockCoords.Y,
            Math.Mod(blockCoords.Z, Chunk.SIDE_LENGTH)
        );

        return chunk.GetLocalBlock(localBlockCoords);
    }

    public BlockData GetBlockData(Vector3 position)
    {
        var type = BlockType.Air;

        float height = WorldGenerator(position) * 8;

        if (position.Y < height)
            type = BlockType.Stone;

        return new BlockData(position, type);
    }


    private BlockCoords LocalToWorld(BlockCoords blockCoords)
    {
        return new BlockCoords(blockCoords.X + Coords.X * Chunk.SIDE_LENGTH, blockCoords.Y, blockCoords.Z + Coords.Y * Chunk.SIDE_LENGTH);
    }
    private void GenerateBlocks()
    {
        for (var x = 0; x < Chunk.SIDE_LENGTH; x++)
        {
            for (var z = 0; z < Chunk.SIDE_LENGTH; z++)
            {
                for (var y = Chunk.MIN_HEIGHT; y < Chunk.MAX_HEIGHT; y++)
                    _blocks.Add(new BlockCoords(x, y, z), GetBlockData(new Vector3(x, y, z)));
            }
        }
    }
    private BlockData GetLocalBlock(BlockCoords blockCoords)
    {
        _blocks.TryGetValue(blockCoords, out var block);
        return block;
    }
    private float WorldGenerator(Vector3 position)
    {
        int scale = 4;
        return noiseGenerator.Noise(position.X / scale, position.Z / scale);
    }
}

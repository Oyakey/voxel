using System;
using System.Collections.Generic;
using Godot;
using Voxel.Utils;

namespace Voxel.Chunk;

public class ChunkData(ChunkCoords coords)
{
    public ChunkCoords Coords { get; private set; } = coords;

    private readonly BlockData[] _blocks = new BlockData[Chunk.BLOCKS_PER_CHUNK];

    public BlockData GetBlock(BlockCoords blockCoords)
    {
        var blockWorldCoords = LocalToWorld(blockCoords);

        // var chunkCoords = new ChunkCoords(
        //     Mathf.FloorToInt(blockWorldCoords.X / Chunk.CHUNK_LENGTH),
        //     Mathf.FloorToInt(blockWorldCoords.Y / Chunk.CHUNK_LENGTH),
        //     Mathf.FloorToInt(blockWorldCoords.Z / Chunk.CHUNK_LENGTH)
        // );
        //
        // var chunk = Main.ChunkGenerator.GetChunk(chunkCoords);
        // if (chunk != null)
        // {
        //     var localBlockCoords = new BlockCoords(
        //         Math.Mod(blockCoords.X, Chunk.CHUNK_LENGTH),
        //         blockCoords.Y,
        //         Math.Mod(blockCoords.Z, Chunk.CHUNK_LENGTH)
        //     );
        //
        //     var block = chunk.GetLocalBlock(localBlockCoords);
        //
        //     if (block != null)
        //         return block;
        // }

        // TODO: This is a temporary implementation to test the rendering of the chunks.
        var height = (int)(Mathf.Sin(blockWorldCoords.X * .1) * 10 - Mathf.Sin(blockWorldCoords.Z * .1) * 10);

        var stone = new BlockData(BlockType.Stone);
        var air = new BlockData(BlockType.Air);
        return blockWorldCoords.Y < height ? stone : air;
    }

    public void Break(BlockCoords block)
    {
        var index = GetIndex(block);
        if (!IsValidIndex(index))
            return;
        _blocks[index] = new BlockData(BlockType.Air);
    }

    public void Place(BlockCoords block)
    {
        var index = GetIndex(block);
        if (!IsValidIndex(index))
            return;
        _blocks[index] = new BlockData(BlockType.Stone);
    }

    public bool CanBreakBlock(BlockCoords block)
    {
        var index = GetIndex(block);
        if (!IsValidIndex(index))
            return false;
        return _blocks[index] == null || _blocks[index].Type != BlockType.Air;
    }

    private static int GetIndex(BlockCoords blockCoords)
    {
        return (blockCoords.X << 8) + (blockCoords.Y << 4) + blockCoords.Z;
    }

    private static bool IsValidIndex(int index)
    {
        return index >= 0 && index < Chunk.BLOCKS_PER_CHUNK;
    }

    private BlockCoords LocalToWorld(BlockCoords blockCoords)
    {
        return new BlockCoords(
                blockCoords.X + Coords.X * Chunk.CHUNK_LENGTH,
                blockCoords.Y + Coords.Y * Chunk.CHUNK_LENGTH,
                blockCoords.Z + Coords.Z * Chunk.CHUNK_LENGTH
        );
    }
}

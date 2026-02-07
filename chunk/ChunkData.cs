using System;
using System.Collections.Generic;
using Godot;
using Voxel.Utils;
using Voxel.World.Save;

namespace Voxel.Chunk;

public class ChunkData(ChunkCoords coords, string saveDir = null)
{
    public ChunkCoords Coords { get; private set; } = coords;
    private BlockData[] _blocks = null;

    public BlockData GetBlock(BlockCoords blockCoords)
    {
        var blockWorldCoords = LocalToWorld(blockCoords);

        // Try to load from save. This should be changed later.
        if (_blocks == null)
        {
            var blocks = Saver.LoadChunk(Coords, saveDir);
            if (blocks != null)
            {
                GD.Print($"Loaded chunk {Coords} from save");
                _blocks = blocks;
            }
        }

        if (_blocks != null)
        {
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
            //         Utils.Math.Mod(blockCoords.X, Chunk.CHUNK_LENGTH),
            //         blockCoords.Y,
            //         Utils.Math.Mod(blockCoords.Z, Chunk.CHUNK_LENGTH)
            //     );
            //
            //     var block = chunk.GetLocalBlock(localBlockCoords);

            var block = GetLocalBlock(blockCoords);

            if (block != null)
            {
                // GD.Print($"Found block at {blockCoords}");
                return block;
            }
        }

        // TODO: This is a temporary implementation to test the rendering of the chunks.
        var height = (int)((Mathf.Sin(blockWorldCoords.X * .1) - Mathf.Sin(blockWorldCoords.Z * .1)) * 7)
            + ((Mathf.Sin(blockWorldCoords.X * .01) - Mathf.Sin(blockWorldCoords.Z * .01)) * 10)
            + ((Mathf.Sin(blockWorldCoords.X * 1) - Mathf.Sin(blockWorldCoords.Z * 1)) * .1) + 2;

        var stone = new BlockData(BlockType.Stone);
        var air = new BlockData(BlockType.Air);
        return blockWorldCoords.Y < height ? stone : air;
    }

    public bool SetBlockAtCoords(BlockCoords blockCoords, BlockData block)
    {
        var index = GetIndex(blockCoords);
        if (!IsValidIndex(index))
            return false;
        _blocks[index] = block;
        return true;
    }

    public BlockData GetLocalBlock(BlockCoords blockCoords)
    {
        var index = GetIndex(blockCoords);
        if (!IsValidIndex(index))
            return null;
        return _blocks[index];
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
        return GetIndex(blockCoords.X, blockCoords.Y, blockCoords.Z);
    }
    private static int GetIndex(int x, int y, int z)
    {
        return (x << 8) + (y << 4) + z;
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

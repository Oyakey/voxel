using System.Collections.Generic;
using Godot;
using Voxel.Utils;

namespace Voxel.Chunk;

public class ChunkData(ChunkCoords coords)
{
    public ChunkCoords Coords { get; private set; } = coords;

    public readonly Dictionary<BlockCoords, BlockData> _blocks = [];

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
        // var height = 0;
        // var height = Math.Mod(blockWorldCoords.Z, 10) - 5;
        // var height = Math.Mod(blockWorldCoords.X + blockWorldCoords.Z, 10) - 5;
        var stone = new BlockData(new Vector3(0, 0, 0), BlockType.Stone);
        var air = new BlockData(new Vector3(0, 0, 0), BlockType.Air);
        return blockWorldCoords.Y < height ? stone : air;
    }

    public void Break(BlockCoords block)
    {
        if (_blocks.ContainsKey(block))
            _blocks[block] = new BlockData(new Vector3(0, 0, 0), BlockType.Air);
        _blocks.TryAdd(block, new BlockData(new Vector3(0, 0, 0), BlockType.Air));
    }
    public void Place(BlockCoords block)
    {
        if (_blocks.ContainsKey(block))
            _blocks[block] = new BlockData(new Vector3(0, 0, 0), BlockType.Stone);
        _blocks.TryAdd(block, new BlockData(new Vector3(0, 0, 0), BlockType.Stone));
    }
    public bool CanBreakBlock(BlockCoords block)
    {
        return !_blocks.ContainsKey(block) || _blocks[block].Type != BlockType.Air;
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

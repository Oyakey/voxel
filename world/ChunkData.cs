using Godot;

namespace Voxel.World;

public class ChunkData(ChunkCoords coords)
{
    public ChunkCoords Coords { get; private set; } = coords;
    private BlockData[] _blocks = null;

    public bool SetBlockAtCoords(BlockCoords blockCoords, BlockData block)
    {
        var index = GetIndex(blockCoords);
        if (!IsValidIndex(index))
            return false;
        _blocks ??= new BlockData[Chunk.BLOCKS_PER_CHUNK];
        _blocks.SetValue(block, index);
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

    public static int GetIndex(BlockCoords blockCoords)
    {
        return GetIndex(blockCoords.X, blockCoords.Y, blockCoords.Z);
    }
    public static int GetIndex(int x, int y, int z)
    {
        if (x < 0 || x >= 16)
            return -1;
        if (y < 0 || y >= 16)
            return -1;
        if (z < 0 || z >= 16)
            return -1;
        return (x << 8) + (y << 4) + z;
    }

    public static bool IsValidCoords(BlockCoords blockCoords)
    {
        return blockCoords.X >= 0 && blockCoords.X < Chunk.CHUNK_LENGTH &&
            blockCoords.Y >= 0 && blockCoords.Y < Chunk.CHUNK_LENGTH &&
            blockCoords.Z >= 0 && blockCoords.Z < Chunk.CHUNK_LENGTH;
    }
    public static bool IsValidIndex(int index)
    {
        return index >= 0 && index < Chunk.BLOCKS_PER_CHUNK;
    }

    public BlockCoords LocalToWorld(BlockCoords blockCoords)
    {
        return new BlockCoords(
            blockCoords.X + Coords.X * Chunk.CHUNK_LENGTH,
            blockCoords.Y + Coords.Y * Chunk.CHUNK_LENGTH,
            blockCoords.Z + Coords.Z * Chunk.CHUNK_LENGTH
        );
    }

    public static ChunkCoords ChunkCoordsFromWorldBlockCoords(BlockCoords worldBlockCoords)
    {
        return new ChunkCoords(
            Mathf.FloorToInt((double)worldBlockCoords.X / Chunk.CHUNK_LENGTH),
            Mathf.FloorToInt((double)worldBlockCoords.Y / Chunk.CHUNK_LENGTH),
            Mathf.FloorToInt((double)worldBlockCoords.Z / Chunk.CHUNK_LENGTH)
        );
    }

    public static BlockCoords WorldBlockCoordsToLocalBlockCoords(BlockCoords worldBlockCoords, ChunkCoords chunkCoords)
    {
        return new BlockCoords(
            worldBlockCoords.X - chunkCoords.X * Chunk.CHUNK_LENGTH,
            worldBlockCoords.Y - chunkCoords.Y * Chunk.CHUNK_LENGTH,
            worldBlockCoords.Z - chunkCoords.Z * Chunk.CHUNK_LENGTH
        );
    }
}

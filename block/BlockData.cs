using Godot;

namespace Voxel;

public class BlockData(BlockType type = BlockType.Air)
{
    public BlockType Type { get; } = type;
}

public enum BlockType
{
    Stone,
    Air,
    Dirt,
    Water,
}

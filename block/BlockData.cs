using Godot;

namespace Voxel;

public class BlockData(Vector3 position, BlockType type = BlockType.Air)
{
    public Vector3 Position { get; } = position;
    public BlockType Type { get; } = type;
}

public enum BlockType
{
    Stone,
    Air,
    Dirt,
    Water,
}

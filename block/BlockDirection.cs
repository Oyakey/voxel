namespace Voxel.Blocks;

public enum BlockDirection : int
{
    // The values are multiples of 2 so that we can use them as bit flags.
    // Towards negative Z
    North = 0b_0000_0001,
    // Towards positive Z
    South = 0b_0000_0010,
    // Towards positive X
    East = 0b_0000_0100,
    // Towards negative X
    West = 0b_0000_1000,
    // Towards positive Y
    Up = 0b_0001_0000,
    // Towards negative Y
    Down = 0b_0010_0000
}

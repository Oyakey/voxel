using Godot;

namespace Voxel.Blocks;

public class BlockColor
{
    public Color Top { get; }
    public Color West { get; }
    public Color East { get; }
    public Color North { get; }
    public Color South { get; }
    public Color Down { get; }

    public BlockColor(Color top, Color west, Color east, Color north, Color south, Color down)
    {
        Top = top;
        West = west;
        East = east;
        North = north;
        South = south;
        Down = down;
    }
    public BlockColor(Color texture)
    {
        Top = texture;
        West = texture;
        East = texture;
        North = texture;
        South = texture;
        Down = texture;
    }
    public BlockColor(Color top, Color side, Color down)
    {
        Top = top;
        West = side;
        East = side;
        North = side;
        South = side;
        Down = down;
    }
    public Color GetDirectionColor(BlockDirection direction) => direction switch
    {
        BlockDirection.South => South,
        BlockDirection.North => North,
        BlockDirection.East => East,
        BlockDirection.West => West,
        BlockDirection.Up => Top,
        BlockDirection.Down => Down,
        _ => Top
    };
}

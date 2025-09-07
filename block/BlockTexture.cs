using Godot;

namespace Voxel.Blocks;

public class BlockTexture
{
    public Vector2I Top { get; }
    public Vector2I West { get; }
    public Vector2I East { get; }
    public Vector2I North { get; }
    public Vector2I South { get; }
    public Vector2I Down { get; }

    public BlockTexture(Vector2I top, Vector2I west, Vector2I east, Vector2I north, Vector2I south, Vector2I down)
    {
        Top = top;
        West = west;
        East = east;
        North = north;
        South = south;
        Down = down;
    }
    public BlockTexture(Vector2I texture)
    {
        Top = texture;
        West = texture;
        East = texture;
        North = texture;
        South = texture;
        Down = texture;
    }
    public BlockTexture(Vector2I top, Vector2I side, Vector2I down)
    {
        Top = top;
        West = side;
        East = side;
        North = side;
        South = side;
        Down = down;
    }
    public Vector2I GetDirectionTexture(BlockDirection direction) => direction switch
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

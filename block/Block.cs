using Godot;

public partial class Block : StaticBody3D
{
    private static readonly CompressedTexture2D texture = ResourceLoader
      .Load<CompressedTexture2D>("res://resources/images/block/stone.png");

    private static readonly PackedScene blockPrefab = ResourceLoader
      .Load<PackedScene>("res://block/block.tscn");

    public static Node3D RenderBlock(BlockData blockData, int renderMode = 0)
    {
        var block = blockPrefab.Instantiate<Node3D>();
        block.Position = blockData.Position;
        Main.Chunk.AddChild(block);
        return block;
    }

    public static BlockData GetBlock(Vector3 position)
    {
        return new BlockData(position);
    }

    private static void SetBlockTexture(MeshInstance3D mesh, CompressedTexture2D tex)
    {
        Material surface = mesh.GetSurfaceOverrideMaterial(0);
        surface.Set("shader_parameter/block", tex);
    }

    private static void RenderFace(MeshInstance3D mesh)
    {
        SetBlockTexture(mesh, texture);
        mesh.Visible = true;
    }

    // These meshes correspond to the faces of the block
    // North is the face facing z-
    private MeshInstance3D _northMesh;
    // South is the face facing z+
    private MeshInstance3D _southMesh;
    // East is the face facing x+
    private MeshInstance3D _eastMesh;
    // West is the face facing x-
    private MeshInstance3D _westMesh;
    // Up is the face facing y+
    private MeshInstance3D _upMesh;
    // Down is the face facing y-
    private MeshInstance3D _downMesh;

    private void _ready()
    {
        _northMesh = GetNode<MeshInstance3D>("North");
        _southMesh = GetNode<MeshInstance3D>("South");
        _eastMesh = GetNode<MeshInstance3D>("East");
        _westMesh = GetNode<MeshInstance3D>("West");
        _upMesh = GetNode<MeshInstance3D>("Up");
        _downMesh = GetNode<MeshInstance3D>("Down");

        _northMesh.Visible = false;
        _southMesh.Visible = false;
        _eastMesh.Visible = false;
        _westMesh.Visible = false;
        _upMesh.Visible = false;
        _downMesh.Visible = false;
    }

    public void RenderFaces(int renderMode)
    {
        if ((renderMode & (int)RenderMode.North) != 0)
            RenderFace(_northMesh);
        if ((renderMode & (int)RenderMode.South) != 0)
            RenderFace(_southMesh);
        if ((renderMode & (int)RenderMode.East) != 0)
            RenderFace(_eastMesh);
        if ((renderMode & (int)RenderMode.West) != 0)
            RenderFace(_westMesh);
        if ((renderMode & (int)RenderMode.Up) != 0)
            RenderFace(_upMesh);
        if ((renderMode & (int)RenderMode.Down) != 0)
            RenderFace(_downMesh);
    }

    // The values are multiples of 2 so that we can use them as bit flags.
    public enum RenderMode : int
    {
        North = 0b_0000_0001,
        South = 0b_0000_0010,
        East = 0b_0000_0100,
        West = 0b_0000_1000,
        Up = 0b_0001_0000,
        Down = 0b_0010_0000
    }

}

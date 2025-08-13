using Godot;

public partial class Block : StaticBody3D
{
    private static readonly CompressedTexture2D texture = ResourceLoader
      .Load<CompressedTexture2D>("res://resources/images/block/stone.png");

    private static readonly PackedScene blockPrefab = ResourceLoader
      .Load<PackedScene>("res://block/block.tscn");

    public static void SpawnBlock(BlockData blockData)
    {
        var block = blockPrefab.Instantiate<Node3D>();
        block.Position = blockData.Position;
        Main.Chunk.AddChild(block);
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

        _upMesh.GetSurfaceOverrideMaterial(0).Set("shader_parameter/block", texture);

        SetBlockTexture(_northMesh, texture);
        SetBlockTexture(_southMesh, texture);
        SetBlockTexture(_eastMesh, texture);
        SetBlockTexture(_westMesh, texture);
        SetBlockTexture(_upMesh, texture);
        SetBlockTexture(_downMesh, texture);

        // _northMesh.Visible = false;
        // _southMesh.Visible = false;
        // _eastMesh.Visible = false;
        // _westMesh.Visible = false;
        // _upMesh.Visible = false;
        // _downMesh.Visible = false;
    }

}

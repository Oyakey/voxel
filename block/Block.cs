using Godot;

namespace Voxel.Blocks;

public partial class Block : StaticBody3D
{
    private static readonly CompressedTexture2D _texture = ResourceLoader
      .Load<CompressedTexture2D>("res://resources/images/block/stone.png");

    private static readonly PackedScene blockPrefab = ResourceLoader
      .Load<PackedScene>("res://block/block.tscn");

    public int BlockRenderMode { get; set; }

    private bool _hovered = false;

    private BlockData _blockData;

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

    public static void Spawn(Node3D node, BlockData blockData, int renderMode)
    {
        if (renderMode == 0)
            return;
        if (blockData.Type == BlockType.Air)
            return;
        var block = blockPrefab.Instantiate<Block>();
        block.Position = blockData.Position;
        block.SetBlockData(blockData);
        block.BlockRenderMode = renderMode;
        // node.AddChild(block);
        node.CallDeferred("add_child", block);
    }

    private static void SetFaceTexture(MeshInstance3D mesh, CompressedTexture2D tex)
    {
        if (mesh == null)
        {
            return;
        }
        if (mesh.GetSurfaceOverrideMaterialCount() <= 0)
        {
            return;
        }
        Material surface = mesh.GetSurfaceOverrideMaterial(0);
        surface.Set("shader_parameter/block", tex);
    }

    private static void SetFaceHovered(MeshInstance3D mesh, bool hovered)
    {
        if (mesh == null)
            return;

        if (mesh.GetSurfaceOverrideMaterialCount() <= 0)
            return;

        Material surface = mesh.GetSurfaceOverrideMaterial(0);
        surface.Set("shader_parameter/showBorder", hovered);
    }

    private static void RenderFace(MeshInstance3D mesh)
    {
        SetFaceTexture(mesh, _texture);
        mesh.Visible = true;
    }

    public void SetBlockHovered(bool hovered)
    {
        if (_hovered == hovered)
            return;
        _hovered = hovered;
        SetFaceHovered(_northMesh, hovered);
        SetFaceHovered(_southMesh, hovered);
        SetFaceHovered(_eastMesh, hovered);
        SetFaceHovered(_westMesh, hovered);
        SetFaceHovered(_upMesh, hovered);
        SetFaceHovered(_downMesh, hovered);
    }

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

        RenderFaces(BlockRenderMode);
    }

    public void RenderFaces(int renderMode)
    {
        if ((renderMode & (int)BlockDirection.North) != 0)
            RenderFace(_northMesh);
        if ((renderMode & (int)BlockDirection.South) != 0)
            RenderFace(_southMesh);
        if ((renderMode & (int)BlockDirection.East) != 0)
            RenderFace(_eastMesh);
        if ((renderMode & (int)BlockDirection.West) != 0)
            RenderFace(_westMesh);
        if ((renderMode & (int)BlockDirection.Up) != 0)
            RenderFace(_upMesh);
        if ((renderMode & (int)BlockDirection.Down) != 0)
            RenderFace(_downMesh);
    }

    public void SetBlockData(BlockData blockData)
    {
        _blockData = blockData;
    }
}

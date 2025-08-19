using Godot;

public partial class Block : StaticBody3D
{
    private bool _hovered = false;

    private static CompressedTexture2D _texture = ResourceLoader
      .Load<CompressedTexture2D>("res://resources/images/block/dirt.png");

    private BlockData _blockData;

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
        {
            return;
        }
        if (mesh.GetSurfaceOverrideMaterialCount() <= 0)
        {
            return;
        }
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

    public CompressedTexture2D GetBlockTexture()
    {
        return ResourceLoader
      .Load<CompressedTexture2D>("res://resources/images/block/dirt.png");

    }

    public void SetBlockData(BlockData blockData)
    {
        _blockData = blockData;
    }
}

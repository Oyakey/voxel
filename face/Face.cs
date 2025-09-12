using System.Collections.Generic;
using Godot;
namespace Voxel.Face;

public partial class Face : MeshInstance3D
{
    private ArrayMesh _tmpMesh = new();
    private List<Vector3> _vertices = [];
    private List<Vector2> _uvs = new();
    private StandardMaterial3D _material = new();
    private Color _color = new(0.023438f, 0.908447f, 1);

    private float SolveForY(float x, float z)
    {
        return (x * x - z * z) * .577350269f + .5f;
    }

    private void GenerateQuad(Vector3 positionIn, Vector2 size)
    {

        var bottomRight = new Vector3(positionIn.X, SolveForY(positionIn.X, positionIn.Z), positionIn.Z);
        var topLeft = new Vector3(positionIn.X + size.X, SolveForY(positionIn.X + size.X, positionIn.Z + size.Y), positionIn.Z + size.Y);
        var topRight = new Vector3(bottomRight.X, SolveForY(positionIn.X, positionIn.Z + size.Y), topLeft.Z);
        var bottomLeft = new Vector3(topLeft.X, SolveForY(positionIn.X + size.X, positionIn.Z), bottomRight.Z);

        _vertices.Add(topRight);
        _vertices.Add(bottomRight);
        _vertices.Add(bottomLeft);

        _vertices.Add(topRight);
        _vertices.Add(bottomLeft);
        _vertices.Add(topLeft);

        _uvs.Add(new Vector2(0, 0));
        _uvs.Add(new Vector2(0, 1));
        _uvs.Add(new Vector2(1, 1));

        _uvs.Add(new Vector2(0, 0));
        _uvs.Add(new Vector2(0, 1));
        _uvs.Add(new Vector2(1, 1));
    }
    private void _ready()
    {
        GenerateQuad(new Vector3(0, 0, 0), new Vector2(1, 1));
        // create_quad(0, 0, 0);
        // create_quad(1, 0, 0);
        // create_quad(1, 1, 0);
        // create_quad(0, 1, 0);

        _material.AlbedoColor = _color;

        var surfaceTool = new SurfaceTool();

        surfaceTool.Begin(Mesh.PrimitiveType.Triangles);

        surfaceTool.SetMaterial(_material);


        for (var v = 0; v < _vertices.Count; v++)
        {
            surfaceTool.SetColor(_color);
            surfaceTool.SetUV(_uvs[v]);
            surfaceTool.AddVertex(_vertices[v]);
        }

        surfaceTool.Commit(_tmpMesh);

        Mesh = _tmpMesh;
    }
}

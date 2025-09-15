using Godot;
using Voxel.Chunk;

namespace Voxel;

public partial class MeshTest : MeshInstance3D
{
    private void _ready()
    {
        var renderer = new MeshRenderer();

        renderer.GenerateQuad(Vector3.Zero);

        var surfaceArray = renderer.GetSurfaceArray();

        var arrMesh = new ArrayMesh();

        if (arrMesh != null && surfaceArray.Count > 0)
            arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);

        Mesh = arrMesh;
    }
}

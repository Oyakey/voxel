using Godot;
using Voxel.Chunk;

namespace Voxel;

public partial class MeshTest : MeshInstance3D
{
    private void _ready()
    {
        Godot.Collections.Array surfaceArray = [];
        surfaceArray.Resize((int)Mesh.ArrayType.Max);

        var renderer = new MeshRenderer();

        renderer.GenerateQuad(Vector3.Zero);

        surfaceArray[(int)Mesh.ArrayType.Vertex] = renderer.Verts.ToArray();
        // surfaceArray[(int)Mesh.ArrayType.TexUV] = renderer.UVs.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Normal] = renderer.Normals.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Index] = renderer.Indices.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Color] = renderer.Colors.ToArray();

        var arrMesh = new ArrayMesh();

        if (arrMesh != null && surfaceArray.Count > 0)
            arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);

        Mesh = arrMesh;
    }
}

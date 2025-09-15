using Godot;
using System.Collections.Generic;
using System.Linq;
using Voxel.Blocks;

namespace Voxel.Chunk;

public class MeshRenderer
{
    public List<Vector3> Verts = [];
    public List<Vector2> Uvs = [];
    public List<Vector3> Normals = [];
    public List<int> Indices = [];
    public List<Color> Colors = [];

    public Godot.Collections.Array GetSurfaceArray()
    {
        Godot.Collections.Array surfaceArray = [];
        surfaceArray.Resize((int)Mesh.ArrayType.Max);

        surfaceArray[(int)Mesh.ArrayType.Vertex] = Verts.ToArray();
        surfaceArray[(int)Mesh.ArrayType.TexUV] = Uvs.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Normal] = Normals.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Index] = Indices.ToArray();

        return surfaceArray;
    }

    public void GenerateQuad(Vector3 position, BlockDirection direction = BlockDirection.South)
    {
        var index = Verts.Count;

        switch (direction)
        {
            /** 
             * For a square face with 4 vertices (A top left, B top right, C bottom right, D bottom left)
             * We trace the 2 triangles following this order:
             * A, B, C
             * C, D, A
             * The order of the vertices is important, as it defines the winding order of the face.
             **/

            // Towards negative Z
            case BlockDirection.North:
                Verts.Add(position + new Vector3(0, 0, -1));
                Verts.Add(position + new Vector3(1, 0, -1));
                Verts.Add(position + new Vector3(1, 1, -1));
                Verts.Add(position + new Vector3(0, 1, -1));
                Normals.Add(new Vector3(0, 0, -1));
                Normals.Add(new Vector3(0, 0, -1));
                Normals.Add(new Vector3(0, 0, -1));
                Normals.Add(new Vector3(0, 0, -1));
                Uvs.AddRange(GetUVsFromAtlas(new Vector2I(3, 0), new Vector2I(24, 44)));
                Indices.Add(index);
                Indices.Add(index + 1);
                Indices.Add(index + 2);
                Indices.Add(index + 2);
                Indices.Add(index + 3);
                Indices.Add(index);
                break;

            // Towards positive Z
            case BlockDirection.South:
                Verts.Add(position + new Vector3(0, 0, 0));
                Verts.Add(position + new Vector3(1, 0, 0));
                Verts.Add(position + new Vector3(1, 1, 0));
                Verts.Add(position + new Vector3(0, 1, 0));
                Normals.Add(new Vector3(0, 0, 1));
                Normals.Add(new Vector3(0, 0, 1));
                Normals.Add(new Vector3(0, 0, 1));
                Normals.Add(new Vector3(0, 0, 1));
                Uvs.AddRange(GetUVsFromAtlas(new Vector2I(3, 0), new Vector2I(24, 44)));
                Indices.Add(index + 2);
                Indices.Add(index + 1);
                Indices.Add(index);
                Indices.Add(index);
                Indices.Add(index + 3);
                Indices.Add(index + 2);
                break;

            // Towards positive X
            case BlockDirection.East:
                Verts.Add(position + new Vector3(1, 0, 0));   // Bottom-Front  → BL
                Verts.Add(position + new Vector3(1, 0, -1));  // Bottom-Back   → BR
                Verts.Add(position + new Vector3(1, 1, -1));  // Top-Back      → TR
                Verts.Add(position + new Vector3(1, 1, 0));   // Top-Front     → TL
                Normals.Add(new Vector3(1, 0, 0));
                Normals.Add(new Vector3(1, 0, 0));
                Normals.Add(new Vector3(1, 0, 0));
                Normals.Add(new Vector3(1, 0, 0));
                Uvs.AddRange(GetUVsFromAtlas(new Vector2I(3, 0), new Vector2I(24, 44)));
                Indices.Add(index + 2);
                Indices.Add(index + 1);
                Indices.Add(index);
                Indices.Add(index);
                Indices.Add(index + 3);
                Indices.Add(index + 2);
                break;

            // Towards negative X
            case BlockDirection.West:
                Verts.Add(position + new Vector3(0, 0, -1));
                Verts.Add(position + new Vector3(0, 0, 0));
                Verts.Add(position + new Vector3(0, 1, 0));
                Verts.Add(position + new Vector3(0, 1, -1));
                Normals.Add(new Vector3(-1, 0, 0));
                Normals.Add(new Vector3(-1, 0, 0));
                Normals.Add(new Vector3(-1, 0, 0));
                Normals.Add(new Vector3(-1, 0, 0));
                Uvs.AddRange(GetUVsFromAtlas(new Vector2I(3, 0), new Vector2I(24, 44)));
                Indices.Add(index + 2);
                Indices.Add(index + 1);
                Indices.Add(index);
                Indices.Add(index);
                Indices.Add(index + 3);
                Indices.Add(index + 2);
                break;

            // Towards positive Y
            case BlockDirection.Up:
                Verts.Add(position + new Vector3(0, 1, -1));
                Verts.Add(position + new Vector3(1, 1, -1));
                Verts.Add(position + new Vector3(1, 1, 0));
                Verts.Add(position + new Vector3(0, 1, 0));
                Normals.Add(new Vector3(0, 1, 0));
                Normals.Add(new Vector3(0, 1, 0));
                Normals.Add(new Vector3(0, 1, 0));
                Normals.Add(new Vector3(0, 1, 0));
                Uvs.AddRange(GetUVsFromAtlas(new Vector2I(0, 0), new Vector2I(24, 44)));
                Indices.Add(index);
                Indices.Add(index + 1);
                Indices.Add(index + 2);
                Indices.Add(index + 2);
                Indices.Add(index + 3);
                Indices.Add(index);
                break;

            // Towards negative Y
            case BlockDirection.Down:
                Verts.Add(position + new Vector3(1, 0, -1));
                Verts.Add(position + new Vector3(0, 0, -1));
                Verts.Add(position + new Vector3(0, 0, 0));
                Verts.Add(position + new Vector3(1, 0, 0));
                Normals.Add(new Vector3(0, -1, 0));
                Normals.Add(new Vector3(0, -1, 0));
                Normals.Add(new Vector3(0, -1, 0));
                Normals.Add(new Vector3(0, -1, 0));
                Uvs.AddRange(GetUVsFromAtlas(new Vector2I(2, 0), new Vector2I(24, 44)));
                Indices.Add(index);
                Indices.Add(index + 1);
                Indices.Add(index + 2);
                Indices.Add(index + 2);
                Indices.Add(index + 3);
                Indices.Add(index);
                break;
        }

        Colors.Add(Godot.Colors.Red);
        Colors.Add(Godot.Colors.Green);
        Colors.Add(Godot.Colors.Blue);
        Colors.Add(Godot.Colors.Magenta);
    }

    private static Vector2[] GetUVsFromAtlas(Vector2I tileCoords, Vector2I tilesInAtlas)
    {
        float tileWidth = 1f / tilesInAtlas.X;
        float tileHeight = 1f / tilesInAtlas.Y;
        Vector2 offset = new Vector2(tileCoords.X * tileWidth, tileCoords.Y * tileHeight);

        return [
            offset + new Vector2(0, tileHeight),           // Bottom-left
            offset + new Vector2(tileWidth, tileHeight),   // Bottom-right
            offset + new Vector2(tileWidth, 0),            // Top-right
            offset + new Vector2(0, 0)                     // Top-left
        ];
    }
}

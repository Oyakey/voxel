using Godot;
using System.Collections.Generic;
using Voxel.Blocks;

namespace Voxel.Chunk;

public class MeshRenderer
{
    public List<Vector3> Verts = [];
    public List<Vector2> Uvs = [];
    public List<Vector3> Normals = [];
    public List<int> Indices = [];
    public List<Color> Colors = [];

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
                break;

            // Towards positive Z
            case BlockDirection.South:
                Verts.Add(position + new Vector3(0, 1, 0));
                Verts.Add(position + new Vector3(1, 1, 0));
                Verts.Add(position + new Vector3(1, 0, 0));
                Verts.Add(position + new Vector3(0, 0, 0));
                Normals.Add(new Vector3(0, 0, 1));
                Normals.Add(new Vector3(0, 0, 1));
                Normals.Add(new Vector3(0, 0, 1));
                Normals.Add(new Vector3(0, 0, 1));
                break;

            // Towards positive X
            case BlockDirection.East:
                Verts.Add(position + new Vector3(1, 1, 0));
                Verts.Add(position + new Vector3(1, 1, -1));
                Verts.Add(position + new Vector3(1, 0, -1));
                Verts.Add(position + new Vector3(1, 0, 0));
                Normals.Add(new Vector3(1, 0, 0));
                Normals.Add(new Vector3(1, 0, 0));
                Normals.Add(new Vector3(1, 0, 0));
                Normals.Add(new Vector3(1, 0, 0));
                break;

            // Towards negative X
            case BlockDirection.West:
                Verts.Add(position + new Vector3(0, 1, -1));
                Verts.Add(position + new Vector3(0, 1, 0));
                Verts.Add(position + new Vector3(0, 0, 0));
                Verts.Add(position + new Vector3(0, 0, -1));
                Normals.Add(new Vector3(-1, 0, 0));
                Normals.Add(new Vector3(-1, 0, 0));
                Normals.Add(new Vector3(-1, 0, 0));
                Normals.Add(new Vector3(-1, 0, 0));
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
                break;
        }

        // Uvs.Add(new Vector2(0, 1));
        // Uvs.Add(new Vector2(1, 1));
        // Uvs.Add(new Vector2(1, 0));
        // Uvs.Add(new Vector2(0, 0));

        Indices.Add(index);
        Indices.Add(index + 1);
        Indices.Add(index + 2);
        Indices.Add(index + 2);
        Indices.Add(index + 3);
        Indices.Add(index + 0);

        Colors.Add(Godot.Colors.Red);
        Colors.Add(Godot.Colors.Red);
        Colors.Add(Godot.Colors.Red);
        Colors.Add(Godot.Colors.Red);
        // Colors.Add(Color.FromHtml("#00ff00"));
        // Colors.Add(Color.FromHtml("#0000ff"));
        // Colors.Add(Color.FromHtml("#ff00ff"));
    }
}

using Godot;
using System.Collections.Generic;
using Voxel.Blocks;

namespace Voxel.Chunk;

public class MeshRenderer
{
    private readonly List<Vector3> _vertices = [];
    private readonly List<Vector2> _uvs = [];
    private readonly List<Vector3> _normals = [];
    private readonly List<int> _indices = [];

    public Godot.Collections.Array GetSurfaceArray()
    {
        Godot.Collections.Array surfaceArray = [];
        surfaceArray.Resize((int)Mesh.ArrayType.Max);

        surfaceArray[(int)Mesh.ArrayType.Vertex] = _vertices.ToArray();
        surfaceArray[(int)Mesh.ArrayType.TexUV] = _uvs.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Normal] = _normals.ToArray();
        surfaceArray[(int)Mesh.ArrayType.Index] = _indices.ToArray();

        return surfaceArray;
    }

    public void Clear()
    {
        _vertices.Clear();
        _uvs.Clear();
        _normals.Clear();
        _indices.Clear();
    }

    public readonly Vector2I grassBlockTop = new(8, 40);
    // public readonly Vector2I grassBlockTop = new(38, 36);
    public readonly Vector2I grassBlockSide = new(6, 40);
    // public readonly Vector2I grassBlockSide = new(22, 36);
    public readonly Vector2I dirt = new(7, 40);
    // public readonly Vector2I dirt = new(11, 32);
    public readonly Vector2I atlasSize = new(128, 64);

    public void GenerateQuad(Vector3 position, BlockDirection direction = BlockDirection.South)
    {
        var index = _vertices.Count;

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
                _vertices.AddRange([
                    position + new Vector3(0, 0, 0),
                    position + new Vector3(1, 0, 0),
                    position + new Vector3(1, 1, 0),
                    position + new Vector3(0, 1, 0),
                ]);
                _normals.AddRange([
                    new Vector3(0, 0, -1),
                    new Vector3(0, 0, -1),
                    new Vector3(0, 0, -1),
                    new Vector3(0, 0, -1),
                ]);
                _uvs.AddRange(GetUVsFromAtlas(grassBlockSide, atlasSize));
                _indices.AddRange([
                    index,
                    index + 1,
                    index + 2,
                    index + 2,
                    index + 3,
                    index
                ]);
                break;

            // Towards positive Z
            case BlockDirection.South:
                _vertices.AddRange([
                    position + new Vector3(0, 0, 1),
                    position + new Vector3(1, 0, 1),
                    position + new Vector3(1, 1, 1),
                    position + new Vector3(0, 1, 1),
                ]);
                _normals.AddRange([
                    new Vector3(0, 0, 1),
                    new Vector3(0, 0, 1),
                    new Vector3(0, 0, 1),
                    new Vector3(0, 0, 1),
                ]);
                _uvs.AddRange(GetUVsFromAtlas(grassBlockSide, atlasSize));
                _indices.AddRange([
                    index + 2,
                    index + 1,
                    index,
                    index,
                    index + 3,
                    index + 2,
                ]);
                break;

            // Towards positive X
            case BlockDirection.East:
                _vertices.Add(position + new Vector3(1, 0, 1));   // Bottom-Front  → BL
                _vertices.Add(position + new Vector3(1, 0, 0));  // Bottom-Back   → BR
                _vertices.Add(position + new Vector3(1, 1, 0));  // Top-Back      → TR
                _vertices.Add(position + new Vector3(1, 1, 1));   // Top-Front     → TL
                _normals.AddRange([
                    new Vector3(1, 0, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(1, 0, 0),
                    new Vector3(1, 0, 0),
                ]);
                _uvs.AddRange(GetUVsFromAtlas(grassBlockSide, atlasSize));
                _indices.AddRange([
                    index + 2,
                    index + 1,
                    index,
                    index,
                    index + 3,
                    index + 2,
                ]);
                break;

            // Towards negative X
            case BlockDirection.West:
                _vertices.Add(position + new Vector3(0, 0, 0));
                _vertices.Add(position + new Vector3(0, 0, 1));
                _vertices.Add(position + new Vector3(0, 1, 1));
                _vertices.Add(position + new Vector3(0, 1, 0));
                _normals.AddRange([
                    new Vector3(-1, 0, 0),
                    new Vector3(-1, 0, 0),
                    new Vector3(-1, 0, 0),
                    new Vector3(-1, 0, 0),
                ]);
                _uvs.AddRange(GetUVsFromAtlas(grassBlockSide, atlasSize));
                _indices.AddRange([
                    index + 2,
                    index + 1,
                    index,
                    index,
                    index + 3,
                    index + 2,
                ]);
                break;

            // Towards positive Y
            case BlockDirection.Up:
                _vertices.Add(position + new Vector3(0, 1, 0));
                _vertices.Add(position + new Vector3(1, 1, 0));
                _vertices.Add(position + new Vector3(1, 1, 1));
                _vertices.Add(position + new Vector3(0, 1, 1));
                _normals.AddRange([
                    new Vector3(0, 1, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(0, 1, 0),
                    new Vector3(0, 1, 0),
                ]);
                _uvs.AddRange(GetUVsFromAtlas(grassBlockTop, atlasSize));
                _indices.AddRange([
                    index,
                    index + 1,
                    index + 2,
                    index + 2,
                    index + 3,
                    index
                ]);
                break;

            // Towards negative Y
            case BlockDirection.Down:
                _vertices.Add(position + new Vector3(1, 0, 0));
                _vertices.Add(position + new Vector3(0, 0, 0));
                _vertices.Add(position + new Vector3(0, 0, 1));
                _vertices.Add(position + new Vector3(1, 0, 1));
                _normals.AddRange([
                    new Vector3(0, -1, 0),
                    new Vector3(0, -1, 0),
                    new Vector3(0, -1, 0),
                    new Vector3(0, -1, 0),
                ]);
                _uvs.AddRange(GetUVsFromAtlas(dirt, atlasSize));
                _indices.AddRange([
                    index,
                    index + 1,
                    index + 2,
                    index + 2,
                    index + 3,
                    index
                ]);
                break;
        }
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

using Godot;
using System;

public partial class Quad : MeshInstance3D
{
    public override void _Ready()
    {
        // Create a new ArrayMesh
        ArrayMesh mesh = new ArrayMesh();

        // Define the quad vertices (in 3D space)
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, 0), // Bottom Left
            new Vector3(0.5f, -0.5f, 0),  // Bottom Right
            new Vector3(0.5f, 0.5f, 0),   // Top Right
            new Vector3(-0.5f, 0.5f, 0),  // Top Left
        };

        // Define UV coordinates
        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0, 1), // Bottom Left
            new Vector2(1, 1), // Bottom Right
            new Vector2(1, 0), // Top Right
            new Vector2(0, 0), // Top Left
        };

        // Define the triangle indices
        int[] indices = new int[]
        {
            0, 1, 2, // First triangle
            2, 3, 0  // Second triangle
        };

        // Define normals (pointing forward in +Z)
        Vector3[] normals = new Vector3[]
        {
            Vector3.Forward,
            Vector3.Forward,
            Vector3.Forward,
            Vector3.Forward
        };

        // Create the arrays for the mesh
        Godot.Collections.Array meshArrays = new Godot.Collections.Array();
        meshArrays.Resize((int)Mesh.ArrayType.Max);

        meshArrays[(int)Mesh.ArrayType.Vertex] = vertices;
        meshArrays[(int)Mesh.ArrayType.TexUV] = uvs;
        meshArrays[(int)Mesh.ArrayType.Normal] = normals;
        meshArrays[(int)Mesh.ArrayType.Index] = indices;

        // Add the surface to the mesh
        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, meshArrays);

        // Assign the mesh to the MeshInstance3D
        this.Mesh = mesh;

        // Optional: Add a material
        var material = new StandardMaterial3D();
        material.AlbedoColor = new Color(0.8f, 0.2f, 0.2f);
        this.SetSurfaceOverrideMaterial(0, material);
    }
}

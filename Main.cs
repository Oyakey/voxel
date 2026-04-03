using Godot;
using Voxel.World;

namespace Voxel;

public partial class Main : Node3D
{
	private static ChunkGenerator _chunkGenerator;
	private static Node3D _chunkParent;

	public static ChunkGenerator ChunkGenerator => _chunkGenerator;

	public static ChunkCoords PlayerCurrentChunk { get; set; }
	public static CharacterBody3D Player { get; set; }

	public const int RenderDistance = 1;

	private void _ready()
	{
		var chunkParent = GetNode<Node3D>("Chunks");
		_chunkParent = chunkParent;
		_chunkGenerator = new ChunkGenerator(chunkParent, RenderDistance, new ChunkCache());
	}
}

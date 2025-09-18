using Godot;
using NUnit.Framework;

namespace Voxel.Tests;

public class ChunkTest
{
    [SetUp]
    public void Setup()
    {
        System.Console.WriteLine("Testing chunks:");
    }

    [Test]
    public void Tests()
    {
        HitToBlockCoordsTests();
        Assert.Pass();
    }

    private void HitToBlockCoordsTests()
    {
        TestHitToBlockCoords(
            new Vector3(.2f, 1.5f, 0.0032f),
            new Vector3(0, 0, 1),
            new Vector3I(0, 1, 0)
        );

        TestHitToBlockCoords(
            new Vector3(.2f, 1.5f, -0.0032f),
            new Vector3(0, 0, 1),
            new Vector3I(0, 1, 0)
        );

        TestHitToBlockCoords(
            new Vector3(.2f, 1.5f, -0.0032f),
            new Vector3(0, 1, 0),
            new Vector3I(0, 2, -1)
        );
    }

    private void TestHitToBlockCoords(
        Vector3 hit,
        Vector3 normal,
        Vector3I expectedCoords
    )
    {
        System.Console.WriteLine($"Hit: {hit}, Normal: {normal} should resolve to {expectedCoords}");
        var blockCoords = HitToBlockCoords(hit, normal);
        Assert.AreEqual(expectedCoords, blockCoords);
    }

    private static Vector3I HitToBlockCoords(Vector3 hit, Vector3 normal)
    {
        return new Vector3I(
            normal.X == 0 ? Mathf.FloorToInt(hit.X) : Mathf.RoundToInt(hit.X),
            normal.Y == 0 ? Mathf.FloorToInt(hit.Y) : Mathf.RoundToInt(hit.Y),
            normal.Z == 0 ? Mathf.FloorToInt(hit.Z) : Mathf.RoundToInt(hit.Z)
        );
    }
}

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
        var blockCoords = Player.HitToBlockCoords(hit, normal);
        Assert.That(blockCoords, Is.EqualTo(expectedCoords));
    }
}

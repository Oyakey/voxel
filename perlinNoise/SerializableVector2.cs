public partial class SerializableVector2(float X, float Y)
{
    public float X { get; } = X;
    public float Y { get; } = Y;
    public override string ToString() => $"({X}, {Y})";
}

namespace Voxel.Utils;

public static class Math
{
    public static int Mod(int a, int n)
    {
        return ((a % n) + n) % n;
    }
}

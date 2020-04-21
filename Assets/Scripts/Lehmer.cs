using UnityEngine;

public static class Lehmer
{
    private const int a = 16807;
    private const int m = 2147483647;
    private const int q = 127773;
    private const int r = 2836;

    private static int seed = Random.Range(0, int.MaxValue);

    public static double Next()
    {
        int hi = seed / q;
        int lo = seed % q;
        seed = (a * lo) - (r * hi);
        if (seed <= 0)
            seed += m;
        return (seed * 1.0) / m;
    }
}
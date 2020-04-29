using System.Runtime.CompilerServices;
using UnityEngine;

public static class PerlinNoise
{

    /// <summary>
    /// Sqrt of two constant to better approximate noise
    /// </summary>
    private const float Sqrt2 = 1.414213562f;

    /// <summary>
    /// 2D gradient lookup table
    /// </summary>
    private static readonly Vector2[] gradients = {
            new Vector2( 1f, 0f),
            new Vector2(-1f, 0f),
            new Vector2( 0f, 1f),
            new Vector2( 0f,-1f),
            new Vector2( 1f, 1f).normalized,
            new Vector2(-1f, 1f).normalized,
            new Vector2( 1f,-1f).normalized,
            new Vector2(-1f,-1f).normalized
        };

    /// <summary>
    /// <para>Returns 2d noise</para>
    /// </summary>
    /// <param name="x">x noise point</param>
    /// <param name="y">y noise point</param>
    /// <param name="seed">noise seed</param>
    /// <returns>2d noise</returns>
    public static float Noise(float x, float y, int seed)
    {
        int ix0 = x > 0 ? (int)x : (int)x - 1;
        int iy0 = y > 0 ? (int)y : (int)y - 1;

        float tx0 = x - ix0;
        float ty0 = y - iy0;
        float tx1 = tx0 - 1f;
        float ty1 = ty0 - 1f;

        int ix1 = ix0 + 1;
        int iy1 = iy0 + 1;

        int h0 = Hash(ix0, seed);
        int h1 = Hash(ix1, seed);

        Vector2 g00 = gradients[Hash(h0 + iy0, seed) & 7];
        Vector2 g10 = gradients[Hash(h1 + iy0, seed) & 7];
        Vector2 g01 = gradients[Hash(h0 + iy1, seed) & 7];
        Vector2 g11 = gradients[Hash(h1 + iy1, seed) & 7];

        float v00 = g00.x * tx0 + g00.y * ty0;
        float v10 = g10.x * tx1 + g10.y * ty0;
        float v01 = g01.x * tx0 + g01.y * ty1;
        float v11 = g11.x * tx1 + g11.y * ty1;

        float tx = Smooth(tx0);
        float ty = Smooth(ty0);

        float a = v00;
        float b = v10 - v00;
        float c = v01 - v00;
        float d = v11 - v01 - v10 + v00;

        float noise = a + b * tx + (c + d * tx) * ty;

        return noise * Sqrt2;
    }

    /// <summary>
    /// <para>Returns 1d noise</para>
    /// </summary>
    /// <param name="x">noise position</param>
    /// <param name="seed">noise seed</param>
    /// <returns>1d noise</returns>
    public static float Noise(float x, int seed)
    {
        int ix = x > 0 ? (int)x : (int)x - 1;

        x -= ix;

        int h0 = Hash(ix, seed);
        int h1 = Hash(ix + 1, seed);

        float gx0 = Grad(h0, x);
        float gx1 = Grad(h1, x - 1);

        float tx = Smooth(x);

        float noise = gx0 + tx * (gx1 - gx0);

        return noise * Sqrt2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Hash(int x, int key)
    {
        x = (x ^ key) + (x << 4);
        x = x ^ (x >> 10);
        x = x + (x << 7);
        return x ^ (x >> 13);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Smooth(float t)
    {
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float SmoothDerivative(float t)
    {
        return 30f * t * t * (t * (t - 2f) + 1f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Grad(int hash, float x)
    {
        return (hash & 1) == 0 ? x : -x;
    }

}

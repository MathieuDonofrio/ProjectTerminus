using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GunController))]
public class SprayPattern : MonoBehaviour
{
    /* Configuration */

    [Tooltip("Threshold value for recoil bias")]
    public float topThreshold = 0.6f;

    [Tooltip("The bias used to determine y axis direction")]
    public float upBias = 0.5f;

    [Tooltip("Value multiplied to final x axis recoil result")]
    public float xAxisModifer = 0.5f;

    [Tooltip("Value multiplied to final y axis recoil result")]
    public float yAxisModifer = 1.0f;

    [Tooltip("Seed used in pseudo random number generators")]
    public int seed;

    /* Required Components */

    private GunController gunController;

    /* State */

    [Header("Debug")]

    public Vector2[] values;

    private void Start()
    {
        gunController = GetComponent<GunController>();

        ComputeValues();
    }

    private void OnDestroy()
    {
        values = null;
    }

    /* Services */

    /// <summary>
    /// Computes entire spray pattern and stores it. Called once at start.
    /// </summary>
    public void ComputeValues()
    {
        System.Random random = new System.Random(seed);

		int max = gunController.maxClipSize;

        // New array
		values = new Vector2[max];

        // X axis frequencies
		float xf1 = (float) random.NextDouble() * 0.2f + 0.2f;
		float xf2 = (float) random.NextDouble() * 0.06f + 0.02f;

        // Y axis frequencies
		float yf1 = (float) random.NextDouble() * 0.15f + 0.2f;
		float yf2 = (float) random.NextDouble() * 0.3f + 1f;

        // Sum
		float xsum = 0;
		float ysum = 0;

		bool xflip = false;
		bool yflip = false;

		int stateSwitchShot = (int)(max * topThreshold);

		for (int i = 0; i < max; ++i)
		{
            // Check threashold
            bool threashhold = i < stateSwitchShot;

            // Gradient noise
            float x = PerlinNoise.Noise(i * (threashhold ? xf1 : xf2), seed);
            float y = PerlinNoise.Noise(i * (threashhold ? yf1 : yf2), seed);

            // More noise 
            x *= 1 + 0.5f * (float) random.NextDouble();
            y *= 1 + 0.5f * (float) random.NextDouble();

            // Threasholding
            if (threashhold)
            {
                x *= 0.75f;
                y = Mathf.Abs(y) * 1.25f;
            }
            else
            {
                y *= 0.5F;
            }

            // Apply up bias
            y = Mathf.Lerp(y, Mathf.Abs(y), upBias);

            // Apply modifiers
            x *= xAxisModifer;
            y *= yAxisModifer;

            // Bound flipping
            if (xflip)
                x = -x;

            if (yflip)
                y = -y;

            if ((x > 0 && xsum > 90) || (x < 00 && xsum < -9))
                xflip = !xflip;

            if ((y > 0 && ysum > 19) || (y < 00 && ysum < 9))
                yflip = !yflip;

            // Add to values
            values[i] = new Vector2(x, y);

            // Calculate sum
            xsum += x;
            ysum += y;

        }
	}

    /// <summary>
    /// Returns the recoil value for the given consecutive shot
    /// </summary>
    /// <param name="shot">consecutive shot</param>
    /// <returns>recoil for consecutive shot</returns>
    public Vector2 getRecoil(int shot)
    {
        return values[Mathf.Clamp(shot, 0, gunController.maxClipSize)];
    }
}

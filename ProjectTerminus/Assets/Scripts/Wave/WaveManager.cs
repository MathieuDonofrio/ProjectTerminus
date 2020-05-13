using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaveManager : MonoBehaviour
{
    /* Configuration */

    [Tooltip("The player tranform to use as center of system")]
    public Transform playerTransform;

    [Tooltip("The zombie prefab used to instantiate new zombies")]
    public ZombieController zombiePrefab;

    [Tooltip("The controller used to update the hud")]
    public HUDController hudController;

    [Tooltip("The starting spawnrate")]
    public float startingSpawnRate = 6.0f;

    /* State */

    public bool IsGameOver { get; set; }

    private Queue<ZombieController> inactive;

    private List<ZombieController> active;

    private float spawnDelta;

    private int queuedSpawns;

    private int wave;

    private void Start()
    {
        inactive = new Queue<ZombieController>();
        active = new List<ZombieController>();

        IsGameOver = false;

        wave = 1;

        queuedSpawns = 5;

        hudController.UpdateWave(wave, true);
    }

    private void FixedUpdate()
    {
        HandleZombies();

        if(queuedSpawns > 0)
        {
            float spawnrate = startingSpawnRate;

            spawnDelta += Time.fixedDeltaTime;

            if(spawnDelta >= spawnrate)
            {
                Vector3 position = RandomSpawnLocation(playerTransform.position, 10, 2);

                Quaternion rotation = Quaternion.LookRotation(playerTransform.position + Vector3.up);

                ZombieController zombie;

                if (inactive.Count > 0)
                {
                    zombie = inactive.Dequeue();

                    zombie.transform.position = position;

                    zombie.transform.rotation = rotation;

                    zombie.Revive();

                    zombie.gameObject.SetActive(true);

                    zombie.StartSpawnZombie();
                }
                else
                {
                    zombie = Instantiate(zombiePrefab, position, rotation, transform);
                }

                zombie.Enabled = true;

                active.Add(zombie);

                spawnDelta -= spawnrate;

                queuedSpawns--;

            }
        }

        if(queuedSpawns <= 0)
        {
            NextWave();
        }

    }

    private void HandleZombies()
    {
        for (int i = active.Count - 1; i >= 0; i--)
        {
            if (active[i].IsDead())
            {
                CheckIn(active[i]);
                active.RemoveAt(i);
            }
        }
    }

    private void CheckIn(ZombieController zombie)
    {
        zombie.Enabled = false;

        zombie.gameObject.SetActive(false);

        inactive.Enqueue(zombie);
    }

    /* Services */

    public void NextWave()
    {
        wave++;

        queuedSpawns = Mathf.CeilToInt(Mathf.Sqrt(30 * wave)) + 5;

        hudController.UpdateWave(wave);
    }

    public Vector3 RandomSpawnLocation(Vector3 center, float max, float min)
    {
        float delta = max - min;

        float range = Mathf.Max(max * 0.25f, 1);

        NavMeshHit hit;

        for(int i = 0; i < 16; ++i)
        {
            Vector3 point = Random.onUnitSphere * (max - delta * Random.value) + center;

            if (NavMesh.SamplePosition(point, out hit, range, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return center;
    }
}

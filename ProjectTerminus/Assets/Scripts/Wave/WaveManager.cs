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

    [Tooltip("Game object container of all road spawn points")]
    public GameObject roadSpawnPointContainer;

    [Tooltip("The starting spawnrate")]
    public float startingSpawnRate = 6.0f;

    /* State */

    public bool IsGameOver { get; set; }

    private Queue<ZombieController> inactive;

    private List<ZombieController> active;

    private SpawnPoint[] roadSpawnPoints;

    private float spawnDelta;

    private float currentRage;

    private int queuedSpawns;

    private int wave;

    private void Start()
    {
        inactive = new Queue<ZombieController>();
        active = new List<ZombieController>();

        roadSpawnPoints = roadSpawnPointContainer.GetComponentsInChildren<SpawnPoint>();

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
            float spawnrate = 0.5f + startingSpawnRate / (wave * 0.5f);

            spawnDelta += Time.fixedDeltaTime;

            if(spawnDelta >= spawnrate)
            {
                Vector3 position = RandomSpawnPosition();

                Quaternion rotation = Quaternion.LookRotation(playerTransform.position + Vector3.up);

                float rage = 0.75f + currentRage + Random.value * 0.5f;

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

                zombie.rage = rage;

                zombie.Enabled = true;

                active.Add(zombie);

                spawnDelta -= spawnrate;

                queuedSpawns--;
            }
        }

        if(queuedSpawns <= 0 && active.Count == 0)
        {
            NextWave();
        }

        hudController.UpdateZombieCount(queuedSpawns + active.Count);
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

        queuedSpawns = Mathf.CeilToInt(Mathf.Sqrt(50 * wave)) + 10;

        currentRage = Mathf.Max(Mathf.Log(wave), 0);

        hudController.UpdateWave(wave);
    }

    public Vector3 RandomSpawnPosition()
    {
        SpawnPoint spawnPoint = roadSpawnPoints[Random.Range(0, roadSpawnPoints.Length - 1)];

        Vector2 offset = Random.insideUnitCircle * spawnPoint.radius;

        Vector3 position = spawnPoint.transform.position;

        position.x += offset.x;
        position.y += offset.y;

        return position;
    }

    public Vector3 FindOnNavMesh(Vector3 point, float searchRadius)
    {
        if (NavMesh.SamplePosition(point, out NavMeshHit hit, searchRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return point;
    }
}

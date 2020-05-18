using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
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

    [Header("Zombie Settings")]
    [Tooltip("The starting spawnrate")]
    public float startingSpawnRate = 6.0f;

    [Tooltip("The starting amount of zombies")]
    public int startingZombieCount = 5;

    [Header("Skybox Settings")]
    [Tooltip("Skybox material")]
    public Material skyboxMat;

    [Tooltip("Normal color tint")]
    public Color normalTint = Color.white;

    [Tooltip("Color tint when changing waves")]
    public Color nextWaveTint = Color.red;

    public float skyboxEffectDuration = 2;

    [Header("Audio Clips")]
    [Tooltip("Sound played when a new wave is comming")]
    public AudioClip newWaveSFX;

    /* Required Components */

    private AudioSource audioSource;

    /* State */

    public bool IsGameOver { get; set; }

    private Queue<ZombieController> inactive;

    private List<ZombieController> active;

    private SpawnPoint[] roadSpawnPoints;

    private float spawnDelta;

    private float currentRage;

    private int queuedSpawns;

    private float lastWaveChange;

    private bool resetSkybox;

    private int wave;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        inactive = new Queue<ZombieController>();
        active = new List<ZombieController>();

        roadSpawnPoints = roadSpawnPointContainer.GetComponentsInChildren<SpawnPoint>();

        IsGameOver = false;
    }

    private void OnDestroy()
    {
        skyboxMat.SetColor("_Tint", normalTint);

        wave = 0;
    }

    private void LateUpdate()
    {
        float delta = Time.time - lastWaveChange;

        if(delta <= skyboxEffectDuration)
        {
            float t = Mathf.Sin(delta / skyboxEffectDuration * Mathf.PI);

            skyboxMat.SetColor("_Tint", Color.Lerp(normalTint, nextWaveTint, t));

            skyboxMat.SetFloat("_Exposure", 0.5f + t);

            resetSkybox = true;
        }
        else if(resetSkybox)
        {
            skyboxMat.SetColor("_Tint", normalTint);

            skyboxMat.SetFloat("_Exposure", 0.5f);

            resetSkybox = false;
        }
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

        if(wave == 0 || (queuedSpawns <= 0 && active.Count == 0))
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

        queuedSpawns = wave == 1 ? startingZombieCount : Mathf.CeilToInt(Mathf.Sqrt(40 * wave)) + startingZombieCount;

        currentRage = wave == 1 ? 0 : Mathf.Max(Mathf.Log(wave), 0);

        hudController.UpdateWave(wave, wave == 1);

        audioSource.PlayOneShot(newWaveSFX);

        lastWaveChange = Time.time;
    }

    public Vector3 RandomSpawnPosition()
    {
        SpawnPoint spawnPoint = roadSpawnPoints[Random.Range(0, roadSpawnPoints.Length - 1)];

        Vector2 offset = Random.insideUnitCircle * spawnPoint.radius;

        Vector3 position = spawnPoint.transform.position;

        position.x += offset.x;
        position.y += offset.y;

        position = FindOnNavMesh(position, spawnPoint.radius);

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

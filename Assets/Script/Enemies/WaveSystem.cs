using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    /* The purpose of this script is to manage the spawning of enemy waves as the game progresses.
     This script will create waves to be added to a list, determine whether or not the current wave
     has been completed, and then spawn the next wave with increasing numbers of enemies and a faster
     spawn rate, as well as varying enemy types based on how far the player has progressed. */
    
    /* This enum is used to hold the states of the wave system.
    The SPAWNING state is for when the system is actively spawning a wave.
    The WAITING state is for when the system is waiting for the wave to be completed.
    The COUNTDOWN state is for when the system is counting down between the end of
    one wave and the beginning of the next. */
    public enum SpawnState { SPAWNING, WAITING, COUNTDOWN }

    /* The Wave class is used to hold all information relating to a wave, including:
    an integer that designates which wave it is,
    a Transform for the enemy object,
    an integer for the number of enemies in the wave,
    and a float for the rate at which enemies spawn. */
    [System.Serializable]
    public class Wave
    {
        public int waveNumber;
        public List<Transform> enemiesList;
        public int enemyCount;
        public float spawnRate;
    }

    [Header("Audio for completed waves")]
    [SerializeField] private AudioSource waveComplete;
    [SerializeField] private AudioClip waveCompleteClip;

    [SerializeField] private PlayerController player;
    // This list is used to store the waves.
    // public Wave[] waves;
    public List<Wave> wavesList;
    // This integer is used as an index to designate the next wave.
    private int nextWave = 0;

    // This list will hold all active spawn points in the level.
    public List<Transform> spawnPoints = new List<Transform>();

    /* The following floats are used to hold the grace period between each wave (waveGracePeriod)
     and the actual countdown timer that will be decreased between waves (waveCountdown). */
    public float waveGracePeriod;
    public float waveCountdown;

    /* This float holds the rate at which the system should check if there are any enemies left alive.
    TODO: This value is currently hard-coded and should be changed. */
    private float searchCountdown = 1f;

    // The following variable is used to hold the current state of the system.
    private SpawnState state = SpawnState.COUNTDOWN;

    // This float is used to manage the rate at which each wave increases the amount of enemies.
    public float enemyIncreaseRate;

    // This float is used to manage the rate at which the spawn rate increases by wave.
    public float spawnRateIncrease;

    // This array holds all Enemy types to be used when creating a wave.
    public Transform[] enemyPrefabs;

    // This int indicates what the current wave is.
    public int currentWave;

    [SerializeField] private float healthMultiplier;

    private void Start()
    {
        waveComplete = GetComponent<AudioSource>();
        waveCountdown = waveGracePeriod;
        player = FindObjectOfType<PlayerController>();
    }

    public void ResetHealth()
    {
        Enemy enemy = enemyPrefabs[0].GetComponent<Enemy>();
        enemy.maxHealth = enemy.baseMaxHealth;
    }

    private void Update()
    {
        if (state == SpawnState.WAITING)
        {
            if (!EnemyIsAlive())
            {
                WaveCompleted();
                return;
            }
            else
            {
                return;
            }
        }

        // If the countdown has ended...
        if (waveCountdown <= 0)
        {
            // ...and the system is not currently spawning a wave...
            if (state != SpawnState.SPAWNING)
            {
                // ...start spawning the next wave.
                CreateNewWave();
                StartCoroutine(SpawnWave(wavesList[nextWave]));
            }
        }
        else
        {
            // If the countdown has not ended, the countdown is still active.
            waveCountdown -= Time.deltaTime;
        }
    }

    void WaveCompleted()
    {
        waveComplete.PlayOneShot(waveCompleteClip);
        Debug.Log("Wave Completed!");

        state = SpawnState.COUNTDOWN;
        waveCountdown = waveGracePeriod;

        /* if ((nextWave + 1) > (wavesList.Count - 1))
        {
            nextWave = 0;
            Debug.Log("All waves complete! Looping.");
        }
        else
        {
            nextWave++;
        } */
        nextWave++;
    }

    /* This method will return false if all enemies are dead, and true if there are any remaining.
    The method operates on a timer to avoid overloading the system with constant searches. */
    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }

        return true;
    }

    // This IEnumerator will be used to start a coroutine for spawning the next wave.
    IEnumerator SpawnWave(Wave _wave)
    {
        Debug.Log("Spawning Wave: " + _wave.waveNumber);
        currentWave = _wave.waveNumber;

        state = SpawnState.SPAWNING;


        for (int i = 0; i < _wave.enemyCount; i++)
        {
            SpawnEnemy(_wave.enemiesList[i]);
            yield return new WaitForSeconds(1f / _wave.spawnRate);
        }

        state = SpawnState.WAITING;

        yield break;
    }

    /*void SpawnEnemy(List<Transform> _enemies)
    {
        // Cycle through all enemies in list and set spawn point + instantiate
        for (int i = 0; i < _enemies.Count - 1; i++)
        {
            Transform _spawn = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
            Instantiate(_enemies[i], _spawn.position, _spawn.rotation);
        }
    }*/

    void SpawnEnemy(Transform _enemy)
    {
        // Cycle through all enemies in list and set spawn point + instantiate
        Transform _spawn = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Instantiate(_enemy, _spawn.position, _spawn.rotation);
    }

    /* This method is used to generate a new wave based on the rates at which the enemy count and
    spawn rate increase. The wave is then added to the list that stores them and ready to be used. */
    void CreateNewWave()
    {
        Wave newWave = new Wave();
        newWave.waveNumber = nextWave + 1;
        newWave.enemyCount = newWave.waveNumber * (int)enemyIncreaseRate;
        newWave.spawnRate = (float)newWave.waveNumber * spawnRateIncrease;
        newWave.enemiesList = GenerateEnemies(enemyPrefabs, newWave.enemyCount);
        wavesList.Add(newWave);
        Debug.Log($"{newWave.waveNumber}");
        Enemy enemy = enemyPrefabs[0].GetComponent<Enemy>();
        enemy.maxHealth += 50f; /*enemy.maxHealth * newWave.waveNumber * healthMultiplier*/;
        Debug.Log($"{enemy.maxHealth}");

        

    }

    List<Transform> GenerateEnemies(Transform[] _enemies, int count)
    {
        List<Transform> enemies = new List<Transform>();

        for (int i = 0; i < count; i++)
        {
            enemies.Add(_enemies[Random.Range(0, _enemies.Length)]);
        }

        return enemies;
    }

    /*
     * TODO: Completed waves need to be stored in some kind of list to track player progression.
     * The number of waves completed could also be stored in some kind of variable so a whole list isn't used.
     * Waves completed need to be tracked to make sure new enemies are added to the waves when needed.
     */
}

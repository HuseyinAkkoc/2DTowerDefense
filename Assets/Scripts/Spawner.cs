using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public static event Action<int> OnWaveChanged;
    public static event Action OnLevelCompleted;        // trgger after 10th wave

    [Header("Level / Waves")]
    [SerializeField] private LevelData currentLevel;   // contains 10 waves
    private WaveData CurrentWave => currentLevel.waves[_currentWaveIndex];

    private int _currentWaveIndex = 0;
    private int _waveCounter = 0;   // UI wave number (1–10)

    [Header("Spawn Controls")]
    private float _spawnTimer;
    private bool _isBetweenWaves = false;
    private float _timeBetweenWaves = 1.0f;
    private float _waveCooldown;

    private int _enemiesRemoved = 0;
    private int _spawnedInEntry = 0;
    private int _currentEntryIndex = 0;

    // Pools
    [Header("Enemy Pools")]
    [SerializeField] private ObjectPooler zombiePool;
    [SerializeField] private ObjectPooler batPool;
    [SerializeField] private ObjectPooler golemPool;
    [SerializeField] private ObjectPooler thiefPool;
    [SerializeField] private ObjectPooler knightPool;
    [SerializeField] private ObjectPooler soldierPool;

    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;


    private void Awake()
    {
        _poolDictionary = new Dictionary<EnemyType, ObjectPooler>()
        {
            { EnemyType.Zombie, zombiePool },
            { EnemyType.Bat, batPool },
            { EnemyType.Golem, golemPool },
            { EnemyType.Thief, thiefPool },
            { EnemyType.Knight, knightPool },
            { EnemyType.Soldier, soldierPool }
        };
    }

    private void Start()
    {
        OnWaveChanged?.Invoke(_waveCounter);
    }

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
    }

    private void Update()
    {
        if (_isBetweenWaves)
        {
            _waveCooldown -= Time.deltaTime;

            if (_waveCooldown <= 0f)
            {
                MoveToNextWave();
            }
            return;
        }

        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0f)
        {
            SpawnEnemy();
        }

        // If done spawning AND all enemies died
        if (IsWaveFinished())
        {
            StartWaveCooldown();
        }
    }


    private void MoveToNextWave()
    {
        _currentWaveIndex++;

        // End of level reached
        if (_currentWaveIndex >= currentLevel.waves.Length)
        {
            Debug.Log("LEVEL COMPLETE!");
            OnLevelCompleted?.Invoke();
            return;
        }

        // Reset wave data
        _waveCounter++;
        OnWaveChanged?.Invoke(_waveCounter);

        ResetWaveState();
    }

    private void ResetWaveState()
    {
        _spawnTimer = 0f;
        _isBetweenWaves = false;
        _enemiesRemoved = 0;

        _spawnedInEntry = 0;
        _currentEntryIndex = 0;

        if (CurrentWave.waveType == WaveType.Single)
        {
            _spawnTimer = CurrentWave.singleSpawnInterval;
        }
        else
        {
            _spawnTimer = CurrentWave.multiEntries[0].spawnInterval;
        }
    }

    private void StartWaveCooldown()
    {
        _isBetweenWaves = true;
        _waveCooldown = _timeBetweenWaves;
    }


    private void SpawnEnemy()
    {
        WaveData wave = CurrentWave;

        // SINGLE WAVE
        if (wave.waveType == WaveType.Single)
        {
            if (_spawnedInEntry < wave.singleEnemyCount)
            {
                SpawnFromPool(wave.singleEnemyType, wave.healthMultiplier);
                _spawnedInEntry++;
                _spawnTimer = wave.singleSpawnInterval;
            }
            return;
        }

        // MULTIPLE WAVE
        if (_currentEntryIndex >= wave.multiEntries.Count)
            return;

        WaveEntry entry = wave.multiEntries[_currentEntryIndex];

        SpawnFromPool(entry.enemyType, wave.healthMultiplier);
        _spawnedInEntry++;

        if (_spawnedInEntry >= entry.count)
        {
            _currentEntryIndex++;
            _spawnedInEntry = 0;

            if (_currentEntryIndex >= wave.multiEntries.Count)
                return;

            entry = wave.multiEntries[_currentEntryIndex];
        }

        _spawnTimer = entry.spawnInterval;
    }


    private void SpawnFromPool(EnemyType type, float healthMultiplier)
    {
        if (!_poolDictionary.TryGetValue(type, out var pool))
            return;

        GameObject obj = pool.GetPooledObject();
        obj.transform.position = transform.position;
        obj.SetActive(true);

        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.Initialize(healthMultiplier);
        enemy.SetAnimationBoolTrue();
    }


    private bool IsWaveFinished()
    {
        return _enemiesRemoved >= GetTotalEnemiesInWave() &&
               HasFinishedSpawning();
    }

    private int GetTotalEnemiesInWave()
    {
        WaveData wave = CurrentWave;

        if (wave.waveType == WaveType.Single)
            return wave.singleEnemyCount;

        int sum = 0;
        foreach (var e in wave.multiEntries)
            sum += e.count;

        return sum;
    }

    private bool HasFinishedSpawning()
    {
        WaveData wave = CurrentWave;

        if (wave.waveType == WaveType.Single)
            return _spawnedInEntry >= wave.singleEnemyCount;

        return _currentEntryIndex >= wave.multiEntries.Count;
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _enemiesRemoved++;
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        _enemiesRemoved++;
    }
}

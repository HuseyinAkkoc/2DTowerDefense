using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
   
    private float _spawnTimer;
    private float _spawnCounter;
    private int _enemiesRemoved;
   

    [SerializeField] private ObjectPooler zombiePool;
    [SerializeField] private ObjectPooler batPool;
    [SerializeField] private ObjectPooler golemPool;


    [SerializeField] private WaveData[] waves;
    private int _currentWaveindex = 0;
    private WaveData CurrentWave => waves[_currentWaveindex];


    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;

    private void Awake()
    {
        _poolDictionary = new Dictionary<EnemyType, ObjectPooler>()
        {
            { EnemyType.Zombie, zombiePool },
            { EnemyType.Bat, batPool },
            { EnemyType.Golem, golemPool },

        };
    }

    private void Update()
    {
        _spawnTimer -= Time.deltaTime;

        if(_spawnTimer <= 0 && _spawnCounter < CurrentWave.enemiesPerWave)
        {
            _spawnTimer = CurrentWave.spawnInterval;
            SpawnEnemy();
            _spawnCounter++;

        }
        else if( _spawnCounter >= CurrentWave.enemiesPerWave && _enemiesRemoved >= CurrentWave.enemiesPerWave)
        {
             _currentWaveindex = ( _currentWaveindex +1)% waves.Length;
            _spawnCounter = 0;
        }
            
    }

      
    private void SpawnEnemy()
    {

        if(_poolDictionary.TryGetValue(CurrentWave.enemyType, out var pool))
        {
            GameObject spawnedObject = pool.GetPooledObject();
            spawnedObject.transform.position = transform.position;
            spawnedObject.SetActive(true);
        }
      


    }

}

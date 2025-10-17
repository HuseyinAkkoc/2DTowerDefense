using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }
    public static event Action<int> OnLivesChanged;
    public static event Action<int> OnResourcesChanged;
    private int _lives=5;
    private int _resources=200;
    public int Resources => _resources;

    private float _gameSpeed = 1f;
    public float GameSpeed => _gameSpeed;   



    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
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

    private void Start()
    {
        OnLivesChanged?.Invoke(_lives);
        OnResourcesChanged?.Invoke(_resources);
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _lives = Mathf.Max(0,_lives-data.damage);
        OnLivesChanged?.Invoke(_lives);
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        AddResources(Mathf.RoundToInt(enemy.Data.resourceReward));
    }
    private void AddResources(int amount)
    {
        _resources+=amount;
        OnResourcesChanged?.Invoke(_resources);
    }

   public void SetTimeScale(float scale)  // pause or unpause
    {
        Time.timeScale = scale;
    }


    // game speed

    public  void SetGameSpeed(float newSpeed)
    {
        _gameSpeed = newSpeed;
        SetTimeScale(_gameSpeed);
    }


    public void SpendResources(int amount)
    {
        if(_resources!  >= amount)
        {
            _resources -= amount;   
            OnResourcesChanged?.Invoke(_resources);
        }
    }
}

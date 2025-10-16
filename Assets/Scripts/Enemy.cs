using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{

    [SerializeField] private EnemyData data;
    public EnemyData Data => data;
    public static event Action<EnemyData> OnEnemyReachedEnd;    // whenever it triggers  it will pass the enemy data that reached the final point. 
    public static event Action<Enemy> OnEnemyDestroyed;
    private Path _currentPath;


    private Vector3 _targetPosition;
    private int _currentWayPoint;
    private float _lives;
    private float _maxLives;


    [SerializeField] private Transform healthBar;
    private Vector3 _healthBarOriginalScale;




    private void Awake()
    {
        _currentPath = GameObject.Find("Path").GetComponent<Path>();
        _healthBarOriginalScale = healthBar.localScale;

    }


    private void OnEnable()
    {
        if (data == null)
        {
            Debug.LogError("EnemyData not assigned on " + gameObject.name);
            return;
        }


        _currentWayPoint = 0;
            _targetPosition = _currentPath.GetPosition(_currentWayPoint);
            _lives = data.lives;
       // UpdateHealthBar();

        

    }

    

    private void Update()
    {
        if (data == null || _currentPath == null) return;

        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, data.speed * Time.deltaTime);
        float relativeDistance = (_targetPosition - transform.position).magnitude;

        if (relativeDistance < 0.1f)
        {
            if (_currentWayPoint < _currentPath.wayPoints.Length - 1)
            {
                _currentWayPoint++;
                _targetPosition = _currentPath.GetPosition(_currentWayPoint);

            }

            else// our current enemy reach the end.
            {
                OnEnemyReachedEnd?.Invoke(data);    // trigger if not null and reach the last point.
                gameObject.SetActive(false);
            }

        }
    }


    public void TakeDamage(float damage)
    {
        _lives -= damage;    
        _lives = Math.Max(_lives, 0);
        UpdateHealthBar();
        if(_lives <= 0)
        {
            OnEnemyDestroyed?.Invoke(this);
            gameObject.SetActive(false);
        }
    }


    private void UpdateHealthBar()
    {
        float healthPercent = _lives / _maxLives;
        Vector3 scale = _healthBarOriginalScale;
        scale.x = _healthBarOriginalScale.x * healthPercent;
        healthBar.localScale = scale;
    }   


    public void Initialize(float healthMultiplier)
    {
        _maxLives= data.lives * healthMultiplier;
        _lives = _maxLives;
        UpdateHealthBar();

    }
}

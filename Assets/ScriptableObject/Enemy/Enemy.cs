using UnityEngine;
using System;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData data;
    public EnemyData Data => data;

    // OLD EVENTS (still used by your gold/lives logic)
    public static event Action<EnemyData> OnEnemyReachedEnd;
    public static event Action<Enemy> OnEnemyDestroyed;

    // NEW RELIABLE EVENTS FOR SPAWNER
    public static event Action OnEnemySpawned;
    public static event Action OnEnemyDied;

    private Path _currentPath;

    private Vector3 _targetPosition;
    private int _currentWayPoint;

    private float _lives;
    private float _maxLives;

    private bool _hasBeenCounted = false;

    [SerializeField] private Transform healthBar;
    private Vector3 _healthBarOriginalScale;

    public Animator animator;


    // ---------------------------------------------------
    // INITIALIZATION
    // ---------------------------------------------------
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

        // Reset movement
        _currentWayPoint = 0;
        _targetPosition = _currentPath.GetPosition(_currentWayPoint);

        // Reset life
        _hasBeenCounted = false;

        // Notify spawner
        OnEnemySpawned?.Invoke();

        // Reset animation
        animator.SetBool("IsAlive", true);
    }


    // ---------------------------------------------------
    // MOVEMENT
    // ---------------------------------------------------
    private void Update()
    {
        if (data == null || _currentPath == null) return;
        if (_hasBeenCounted) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetPosition,
            data.speed * Time.deltaTime
        );

        float dist = (_targetPosition - transform.position).magnitude;

        if (dist < 0.1f)
        {
            if (_currentWayPoint < _currentPath.wayPoints.Length - 1)
            {
                _currentWayPoint++;
                _targetPosition = _currentPath.GetPosition(_currentWayPoint);
            }
            else
            {
                // Reached end
                _hasBeenCounted = true;

                OnEnemyReachedEnd?.Invoke(data);
                OnEnemyDied?.Invoke();           // counts as death for wave system

                gameObject.SetActive(false);
            }
        }
    }


    // ---------------------------------------------------
    // DAMAGE & DEATH
    // ---------------------------------------------------
    public void TakeDamage(float damage)
    {
        _lives -= damage;
        _lives = Mathf.Max(_lives, 0);

        UpdateHealthBar();

        if (_lives <= 0)
        {
            _hasBeenCounted = true;

            // Old event for your resource reward system
            OnEnemyDestroyed?.Invoke(this);

            // New event for accurate wave tracking
            OnEnemyDied?.Invoke();

            // Play death animation
            animator.SetBool("IsAlive", false);

            // Disable after animation
            StartCoroutine(DeactivateAfterDelay());
        }
    }


    private IEnumerator DeactivateAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);

        // Reset for pool reuse
        animator.SetBool("IsAlive", true);
    }


    // ---------------------------------------------------
    // HEALTH BAR
    // ---------------------------------------------------
    private void UpdateHealthBar()
    {
        float percent = _lives / _maxLives;

        Vector3 scale = _healthBarOriginalScale;
        scale.x = _healthBarOriginalScale.x * percent;

        healthBar.localScale = scale;
    }


    public void Initialize(float healthMultiplier)
    {
        _hasBeenCounted = false;

        _maxLives = data.lives * healthMultiplier;
        _lives = _maxLives;

        UpdateHealthBar();
    }


    public void SetAnimationBoolTrue()
    {
        animator.SetBool("IsAlive", true);
    }

    public void SetAnimationBoolFalse()
    {
        animator.SetBool("IsAlive", false);
    }
}

using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] private EnemyData data;
   

     private Path _currentPath;


    private Vector3 _targetPosition;
    private int _currentWayPoint;




    private void Awake()
    {
        _currentPath = GameObject.Find("Path").GetComponent<Path>();
    }


    private void OnEnable()
    {
        _currentWayPoint = 0;
        _targetPosition = _currentPath.GetPosition(_currentWayPoint);
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

            else
            {
                gameObject.SetActive(false);
            }

        }
    }




}

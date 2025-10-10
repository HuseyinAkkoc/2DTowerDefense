using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Path currentPath;


    private Vector3 _targetPosition;
    private int _currentWayPoint;




    private void Awake()
    {
        currentPath = GameObject.Find("Path").GetComponent<Path>();
    }


    private void OnEnable()
    {
        _currentWayPoint = 0;
        _targetPosition = currentPath.GetPosition(_currentWayPoint);
    }

    

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Time.deltaTime);
        float relativeDistance = (_targetPosition - transform.position).magnitude;

        if (relativeDistance < 0.1f)
        {
            if (_currentWayPoint < currentPath.wayPoints.Length - 1)
            {
                _currentWayPoint++;
                _targetPosition = currentPath.GetPosition(_currentWayPoint);

            }

            else
            {
                gameObject.SetActive(false);
            }

        }
    }




}

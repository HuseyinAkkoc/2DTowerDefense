using UnityEngine;
using UnityEditor;

public class Path : MonoBehaviour
{
    public GameObject[] wayPoints;


    public Vector3 GetPosition(int index)
    {
        return wayPoints[index].transform.position;
    }

    private void OnDrawGizmos()
    {
        if(wayPoints.Length > 0)
        {
            for(int i = 0; i < wayPoints.Length; i++)
            {

                GUIStyle style = new GUIStyle();
                style.fontSize = 12;
                style.normal.textColor = Color.white;   
                style.alignment = TextAnchor.MiddleCenter;
                Handles.Label(wayPoints[i].transform.position + Vector3.up * 0.7f, wayPoints[i].name, style);



                if (i < wayPoints.Length - 1)
                {

                    Gizmos.color = Color.gray;

                    Gizmos.DrawLine(wayPoints[i].transform.position, wayPoints[i + 1].transform.position);

                }
                
            }

        }
    }
}

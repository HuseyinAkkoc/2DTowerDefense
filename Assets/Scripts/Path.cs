using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Path : MonoBehaviour
{
    public GameObject[] wayPoints;

    public Vector3 GetPosition(int index)
    {
        return wayPoints[index].transform.position;
    }

   /* private void OnDrawGizmos()
    {
        if (wayPoints == null || wayPoints.Length == 0)
            return;

        for (int i = 0; i < wayPoints.Length; i++)
        {
#if UNITY_EDITOR
            // Draw waypoint label (Editor only)
            GUIStyle style = new GUIStyle
            {
                fontSize = 12,
                normal = { textColor = Color.white },
                alignment = TextAnchor.MiddleCenter
            };
            Handles.Label(wayPoints[i].transform.position + Vector3.up * 0.7f, wayPoints[i].name, style);
#endif

            // Draw connecting lines (works everywhere)
            if (i < wayPoints.Length - 1)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(wayPoints[i].transform.position, wayPoints[i + 1].transform.position);
            }
        }
    }*/
}

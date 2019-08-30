using UnityEngine;

public class BezierSpline : MonoBehaviour
{
    public Transform obstacle;
    public float obstacleRadius;
    public Vector3[] wayPoints;
    public float wayPointSpacing = .5f;
    public Transform from;
    public Transform to;

    private bool InsideObstacleRadius(Vector3 v)
    {
        return Vector3.Distance(v, obstacle.position) < obstacleRadius;
    }

    public void FromTo(Vector3 from, Vector3 to)
    {
        var length = (int) (Vector3.Distance(from, to) / wayPointSpacing);
        wayPoints = new Vector3[length + 1];
        var direction = (to - from).normalized;

        for (int i = 0; i < length; i++)
        {
            wayPoints[i] = from + direction * i;
            if (InsideObstacleRadius(wayPoints[i]))
            {
                wayPoints[i] = (wayPoints[i] - obstacle.position).normalized * obstacleRadius + obstacle.position;
            }
        }

        wayPoints[length] = to;
    }

    private void OnDrawGizmos()
    {
        if (wayPoints.Length > 0)
        {
            for (int i = 1; i < wayPoints.Length - 1; i++)
            {
                Gizmos.DrawLine(wayPoints[i], wayPoints[i + 1]);
                Gizmos.DrawCube(wayPoints[i], Vector3.one * 0.25f);
            }

            Gizmos.DrawCube(wayPoints[wayPoints.Length - 1], Vector3.one * 0.25f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        FromTo(from.position, to.position);
    }

    // Update is called once per frame
    void Update()
    {
        FromTo(from.position, to.position);
    }
}
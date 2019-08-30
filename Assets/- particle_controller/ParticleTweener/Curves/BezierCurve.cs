using System;
using UnityEngine;

[Serializable]
public struct WayPoint
{
    [SerializeField] public Vector3 position;
    [SerializeField] public Vector3 controlPoint;
}

[Serializable]
public struct Obstacle
{
    [SerializeField] public Vector3 position;
    [SerializeField] public float radius;
}

[Serializable]
public class BezierCurve
{
    [SerializeField] public WayPoint[] Spline;
    [SerializeField] public int _segmentCount;
    [SerializeField] public float _segmentRange;

    private BezierCurve(WayPoint[] spline)
    {
        Spline = spline;
        _segmentCount = spline.Length - 1;
        _segmentRange = 1.0f / _segmentCount;
    }

    private BezierCurve(WayPoint[] spline, int segmentCount, float segmentRange)
    {
        Spline = spline;
        _segmentCount = segmentCount;
        _segmentRange = segmentRange;
    }

    private static float CubicVal(float a, float b, float c, float d, float t)
    {
        return Mathf.Pow(1 - t, 3) * a
               + 3 * (Mathf.Pow(1 - t, 2)) * t * b
               + 3 * (1 - t) * t * t * c
               + t * t * t * d;
    }

    public void GetSegment(int index, out Vector3 posA, out Vector3 controlA, out Vector3 posB, out Vector3 controlB)
    {
        posA = Spline[index].position;
        posB = Spline[index + 1].position;
        controlA = Spline[index].controlPoint;
        controlB = Spline[index + 1].controlPoint;

        if (index == 0) return;

        controlA = posA + (posA - controlA);
    }

    public Vector3 GetPos(float t)
    {
        //if it gets to 1 the values will have an offset from the actual array
        //in this case a rest is better than an if to avoid that error
        t -= 0.0001f;
        var index = (int) (_segmentCount * t);
        var subT = 1.0f - (_segmentRange * (index + 1) - t) / _segmentRange;

        Vector3 posA, posB, controlA, controlB;

        GetSegment(index, out posA, out controlA, out posB, out controlB);

        return new Vector3(
            CubicVal(posA.x, controlA.x, controlB.x, posB.x, subT),
            CubicVal(posA.y, controlA.y, controlB.y, posB.y, subT),
            CubicVal(posA.z, controlA.z, controlB.z, posB.z, subT));
    }

    public static BezierCurve FromTo(Vector3 a, Vector3 b, int resolution)
    {
        var distance = Vector3.Distance(a, b);
        var segment = (b - a).normalized * (distance / resolution);
        var nextPosition = a;
        var wayPoints = new WayPoint[resolution + 1];

        for (int i = 0; i < resolution; i++, nextPosition += segment)
        {
            wayPoints[i].position = nextPosition;
            wayPoints[i].controlPoint = Vector3.up + wayPoints[i].position;
        }

        wayPoints[resolution].position = b;
        wayPoints[resolution].controlPoint = Vector3.back + b;

        return new BezierCurve(wayPoints, resolution, 1.0f / resolution);
    }

    public static BezierCurve FromToAvoidingObstacles(Vector3 a, Vector3 b, Obstacle[] obstacles, int resolution)
    {
        var bezierCurve = FromTo(a, b, resolution);
        for (int i = 0; i < bezierCurve._segmentCount + 1; i++)
        {
            for (int j = 0; j < obstacles.Length; j++)
            {
                if (InsideObstacleRadius(bezierCurve.Spline[i].position, obstacles[j]))
                {
                    bezierCurve.Spline[i].position =
                        (bezierCurve.Spline[i].position - obstacles[j].position).normalized * obstacles[j].radius +
                        obstacles[j].position;
                }
            }
        }

        return bezierCurve;
    }

    public static void MakeFromToAvoidingObstacles(Vector3 a, Vector3 b, Obstacle[] obstacles, BezierCurve bezierCurve)
    {
        for (int i = 0; i < bezierCurve._segmentCount + 1; i++)
        {
            for (int j = 0; j < obstacles.Length; j++)
            {
                if (InsideObstacleRadius(bezierCurve.Spline[i].position, obstacles[j]))
                {
                    bezierCurve.Spline[i].position =
                        (bezierCurve.Spline[i].position - obstacles[j].position).normalized * obstacles[j].radius +
                        obstacles[j].position;
                }
            }
        }
    }

    public static BezierCurve GetEmpty(int resolution)
    {
        return new BezierCurve(new WayPoint[resolution + 1]);
    }

    private static bool InsideObstacleRadius(Vector3 v, Obstacle obstacle)
    {
        return Vector3.Distance(v, obstacle.position) < obstacle.radius;
    }
}
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
        var curve = GetEmpty(resolution);
        curve.MakeFromTo(a, b);

        return curve;
    }

    public static BezierCurve FromToAvoidingObstacles(Vector3 a, Vector3 b, Obstacle[] obstacles, int resolution)
    {
        var curve = GetEmpty(resolution);
        curve.MakeFromToAvoidingObstacles(a, b, obstacles);
        return curve;
    }

    public void MakeFromTo(Vector3 a, Vector3 b)
    {
        var distance = Vector3.Distance(a, b);
//        var direction = (b-a).normalized;
        var segment = (b-a).normalized * (distance / _segmentCount);
        var nextPosition = a;

        for (int i = 0; i < _segmentCount; i++, nextPosition += segment)
        {
            Spline[i].position = nextPosition;
            Spline[i].controlPoint = Vector3.up + Spline[i].position;
        }

        Spline[_segmentCount].position = b;
        Spline[_segmentCount].controlPoint = (a - b).normalized + b;
    }

    public void MakeFromToAvoidingObstacles(Vector3 a, Vector3 b, Obstacle[] obstacles)
    {
        MakeFromTo(a, b);

        for (var i = 1; i < _segmentCount; i++)
        {
            for (var j = 0; j < obstacles.Length; j++)
            {
                if (InsideObstacleRadius(Spline[i].position, obstacles[j]))
                {
                    Spline[i].position =
                        (Spline[i].position - obstacles[j].position).normalized * obstacles[j].radius +
                        obstacles[j].position;
                    Spline[i].controlPoint = Spline[i].position + Vector3.up;
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
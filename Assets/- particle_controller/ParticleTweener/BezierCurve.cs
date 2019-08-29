using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening.Plugins.Options;
using UnityEngine;

[Serializable]
public struct WayPoint
{
    [SerializeField] public Vector3 position;
    [SerializeField] public Vector3 controlPoint;
}

public struct Segment
{
    public WayPoint a;
    public WayPoint b;
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
        
        controlA.x = posA.x - controlA.x;
        controlA.y = posA.y - controlA.y;
        controlA.z = posA.z - controlA.z;
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
        var segment = (a - b) * distance / resolution;
        var nextPosition = a;
        var wayPoints = new WayPoint[resolution + 1];

        for (int i = 0; i < resolution - 1; i++, nextPosition += segment)
        {
            wayPoints[i].position = nextPosition;
            wayPoints[i].controlPoint = Vector3.up;
        }

        wayPoints[resolution].position = b;
        wayPoints[resolution].controlPoint = Vector3.back;

        return new BezierCurve(wayPoints, resolution, 1.0f / resolution);
    }

    public static BezierCurve GetEmpty(int resolution)
    {
        return new BezierCurve(new WayPoint[resolution + 1]);
    }
}
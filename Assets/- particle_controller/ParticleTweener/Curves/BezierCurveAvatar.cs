using System;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurveAvatar : MonoBehaviour
{
    public List<BezierCurve> curves = new List<BezierCurve>(1000);
    public int resolution = 8;
    public Obstacle[] obstacles;
    public List<BezierCurve> actuallyUsedCurves = new List<BezierCurve>(1000);

    void Start()
    {
    }

    public BezierCurve Get(Vector3 from, Vector3 to)
    {
        if (curves.Count == 0)
        {
            return BezierCurve.FromToAvoidingObstacles(from, to, obstacles, resolution);
        }

        var curve = curves[0];
        curves.RemoveAt(0);
//        BezierCurve.MakeFromToAvoidingObstacles(from, to, obstacles, curve);
        actuallyUsedCurves.Add(curve);
        return curve;
    }

    public void Recycle(BezierCurve curve)
    {
        curves.Add(curve);
        actuallyUsedCurves.Remove(curve);
    }
}
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ParticleSplineBehavior))]
public class ParticleSplineBehaviorEditor : Editor
{
    private List<BezierCurve> curves;
    private ParticleSplineBehavior particleSpline;
    private Color lineColor = new Color(.2f, 1, .5f);
    private Color edgeColor = new Color(1, .3f, .3f);
    public bool updated = false;

    void OnEnable()
    {
        particleSpline = (ParticleSplineBehavior) target;
        curves = particleSpline.CurvesOnUse;
    }

    private void OnSceneGUI()
    {
        if (curves.Count == 0)
            return;
        DrawSpline();
    }

    private Vector3 temp;

    void DrawObstacles()
    {
    }

    void DrawSplineStartAndEnd()
    {
    }

    void DrawSpline()
    {
        foreach (var curve in curves)
        {
            for (int i = 0; i < curve._segmentCount; i++)
            {
                Vector3 posA;
                Vector3 posB;
                Vector3 controlA;
                Vector3 controlB;

                curve.GetSegment(i, out posA, out controlA, out posB, out controlB);
                Handles.DrawBezier(posA, posB, controlA, controlB, lineColor, null, 2);
                Handles.color = edgeColor;
                Handles.SphereHandleCap(0, posA, Quaternion.identity, .15f,EventType.Ignore);
            }
        }
    }
}
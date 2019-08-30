using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ParticleSplineBehavior))]
public class ParticleSplineBehaviorEditor : Editor
{
    private List<BezierCurve> curves;
    private ParticleSplineBehavior particleSpline;

    void OnEnable()
    {
        particleSpline = (ParticleSplineBehavior) target;
        curves = particleSpline.CurvesOnUse;
    }

    private void OnSceneGUI()
    {
        if (curves.Count == 0)
            return;
        Draw();
    }

    private Vector3 temp;

    void Draw()
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
                Handles.DrawBezier(posA, posB, controlA, controlB, Color.green, null, 2);

//                Handles.color = Color.green;
                //center handle
//                temp =
//                    Handles.FreeMoveHandle(posA, Quaternion.identity, .25f, Vector3.zero, Handles.SphereHandleCap);
//                if (posA != temp)
//                {
//                    Undo.RecordObject(bezier, "MoveBezierPoint");
//                    curve.Spline[i].position = temp;
//                }

//                Handles.DrawLine(posA, controlA);
//                Handles.DrawLine(posB, controlB);
//
//                Handles.color = Color.red;
                //B control point
//                temp =
//                    Handles.FreeMoveHandle(controlB, Quaternion.identity, .15f, Vector3.zero, Handles.SphereHandleCap);
//                if (controlB != temp)
//                {
//                    Undo.RecordObject(bezier, "MoveBezierPoint");
//                    curve.Spline[i + 1].controlPoint = temp;
//                }

                //A control point
//                if (i > 0)
//                    Handles.color = new Color(1, .5f, .5f, 1);
//                temp =
//                    Handles.FreeMoveHandle(controlA, Quaternion.identity, .15f, Vector3.zero, Handles.SphereHandleCap);
//                if (controlA != temp)
//                {
//                    Undo.RecordObject(bezier, "MoveBezierPoint");
//                    if (i != 0)
//                        curve.Spline[i].controlPoint = posA + (posA - temp);
//                    else curve.Spline[i].controlPoint = temp;
//                }
            }


//            Handles.color = Color.green;
//            temp = Handles.FreeMoveHandle(curve.Spline[curve._segmentCount].position, Quaternion.identity, .25f,
//                Vector3.zero,
//                Handles.SphereHandleCap);
//            if (curve.Spline[curve._segmentCount].position != temp)
//            {
//                Undo.RecordObject(bezier, "MoveBezierPoint");
//                curve.Spline[curve._segmentCount].position = temp;
//            }
        }
    }
}
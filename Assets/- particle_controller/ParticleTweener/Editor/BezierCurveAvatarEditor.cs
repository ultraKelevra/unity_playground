using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierCurveAvatar))]
public class BezierCurveAvatarEditor : Editor
{
    private BezierCurveAvatar bezier;
    private BezierCurve curve;

    private void OnSceneGUI()
    {
        Draw();
    }

    private Vector3 temp;

    void Draw()
    {
        for (int i = 0; i < curve._segmentCount; i++)
        {
            Vector3 posA;
            Vector3 posB;
            Vector3 controlA;
            Vector3 controlB;

            curve.GetSegment(i, out posA, out controlA, out posB, out controlB);
            Handles.DrawBezier(posA, posB, controlA, controlB, Color.green, null, 2);

            Handles.color = Color.green;
            //center handle
            temp =
                Handles.FreeMoveHandle(posA, Quaternion.identity, .25f, Vector3.zero, Handles.SphereHandleCap);
            if (posA != temp)
            {
                Undo.RecordObject(bezier, "MoveBezierPoint");
                curve.Spline[i].position = temp;
            }

            Handles.DrawLine(posA, controlA);
            Handles.DrawLine(posB, controlB);

            //A control point
            Handles.color = Color.red;
            temp =
                Handles.FreeMoveHandle(controlA, Quaternion.identity, .1f, Vector3.zero, Handles.SphereHandleCap);
            if (controlA != temp)
            {
                Undo.RecordObject(bezier, "MoveBezierPoint");
                if (i != 0)
                    curve.Spline[i].controlPoint = posA - temp;
                else curve.Spline[i].controlPoint = temp;
            }

            //B control point
            temp =
                Handles.FreeMoveHandle(controlB, Quaternion.identity, .1f, Vector3.zero, Handles.SphereHandleCap);
            if (controlB != temp)
            {
                Undo.RecordObject(bezier, "MoveBezierPoint");
                curve.Spline[i+1].controlPoint = temp;
            }
        }

        temp = Handles.FreeMoveHandle(curve.Spline[curve._segmentCount].position, Quaternion.identity, .1f,
            Vector3.zero,
            Handles.CylinderHandleCap);
        if (curve.Spline[curve._segmentCount].position != temp)
        {
            Undo.RecordObject(bezier, "MoveBezierPoint");
            curve.Spline[curve._segmentCount].position = temp;
        }
    }

    void OnEnable()
    {
        bezier = (BezierCurveAvatar) target;
        if (bezier.curve == null)
            bezier.curve = BezierCurve.FromTo(Vector3.zero, Vector3.one, 1);

        curve = bezier.curve;
    }
}
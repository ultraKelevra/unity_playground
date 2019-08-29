using UnityEngine;

[ExecuteInEditMode]
public class BezierCurveAvatar : MonoBehaviour
{
    public BezierCurve curve;

    void Start()
    {
        curve = BezierCurve.FromTo(new Vector3(0, 0, 0), new Vector3(4, 4, 4), 2);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(curve.GetPos(0), curve.GetPos(0.1f));
        Gizmos.DrawLine(curve.GetPos(0.1f), curve.GetPos(0.2f));
        Gizmos.DrawLine(curve.GetPos(0.2f), curve.GetPos(0.3f));
        Gizmos.DrawLine(curve.GetPos(0.3f), curve.GetPos(0.4f));
        Gizmos.DrawLine(curve.GetPos(0.4f), curve.GetPos(0.5f));
        Gizmos.DrawLine(curve.GetPos(0.5f), curve.GetPos(0.6f));
        Gizmos.DrawLine(curve.GetPos(0.6f), curve.GetPos(0.7f));
        Gizmos.DrawLine(curve.GetPos(0.7f), curve.GetPos(0.8f));
        Gizmos.DrawLine(curve.GetPos(0.8f), curve.GetPos(0.9f));
        Gizmos.DrawLine(curve.GetPos(0.9f), curve.GetPos(1));
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class RoundWorldAvatar : MonoBehaviour
{
    [Range(0, 100)] public float radius = 1;

    [Range(0, 100)] public float horizontalScale = 5;

    [Range(0, 1)] public float verticalScale = 1;

    public Vector4 worldOffset = new Vector4(15,15,15,1);
    public Vector4 sphereOffset = new Vector4(0,-15,0,1);
    [Range(0,1)] public float coordinateInterpolation = 1;
    [Range(0, 2)] public float pivotRotationX;
    [Range(0, 2)] public float pivotRotationY;
    [Range(0, 2)] public float pivotRotationZ;

    public bool update = false;

    private static readonly int Radius = Shader.PropertyToID("_Radius");
    private static readonly int HorizontalScale = Shader.PropertyToID("_HorizontalScale");
    private static readonly int VerticalScale = Shader.PropertyToID("_VerticalScale");
    private static readonly int WorldOffset = Shader.PropertyToID("_WorldOffset");
    private static readonly int SphereOffset = Shader.PropertyToID("_SphereOffset");
    private static readonly int CoordinateInterpolation = Shader.PropertyToID("_CoordinateInterpolation");
    private static readonly int PivotRotationX = Shader.PropertyToID("_PivotRotationX");
    private static readonly int PivotRotationY = Shader.PropertyToID("_PivotRotationY");
    private static readonly int PivotRotationZ = Shader.PropertyToID("_PivotRotationZ");

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (update)
        {
            Shader.SetGlobalFloat(Radius, radius);
            Shader.SetGlobalFloat(HorizontalScale, horizontalScale);
            Shader.SetGlobalFloat(VerticalScale, verticalScale);
            Shader.SetGlobalVector(WorldOffset, worldOffset);
            Shader.SetGlobalVector(SphereOffset, sphereOffset);
            Shader.SetGlobalFloat(CoordinateInterpolation, coordinateInterpolation);
            Shader.SetGlobalFloat(PivotRotationX, pivotRotationX);
            Shader.SetGlobalFloat(PivotRotationY, pivotRotationY);
            Shader.SetGlobalFloat(PivotRotationZ, pivotRotationZ);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleTweenerUtility))]
public class ParticleSplineBehavior : ParticleTweenerModule
{
    public int resolution = 8;
    public Obstacle[] obstacles;

    private ParticleSystem _pSystem;
    public Stack<BezierCurve> CurvesOnWait;
    public List<BezierCurve> CurvesOnUse;
    public float[] time;
    private float _lifeTime;
    public Vector3 a;
    public Vector3 b = new Vector3(0, 0, 5);

    //aux
    private int i;

    public override void InitializeModule(ParticleTweenerUtility particleTweener)
    {
        _pSystem = GetComponent<ParticleSystem>();
        CurvesOnUse = new List<BezierCurve>(particleTweener.ParticleMaxCount);
        CurvesOnWait = new Stack<BezierCurve>(particleTweener.ParticleMaxCount);
        time = new float[particleTweener.ParticleMaxCount];
        _lifeTime = _pSystem.main.startLifetime.constant;
    }

    private void Pop()
    {
        Debug.Log("Pop");
        if (CurvesOnWait.Count != 0)
            CurvesOnUse.Add(CurvesOnWait.Pop());
        else
        {
            CurvesOnUse.Add(BezierCurve.GetEmpty(resolution));
            time[CurvesOnUse.Count - 1] = 0;
        }
    }

    private void Dump()
    {
        Debug.Log("Dump");

        CurvesOnWait.Push(CurvesOnUse[0]);
        CurvesOnUse.RemoveAt(0);
    }

    public override void UpdateModule(ParticleSystem.Particle[] particles, int count)
    {
        while (count > CurvesOnUse.Count)
            Pop();

        while (count < CurvesOnUse.Count)
            Dump();

        for (i = 0; i < count; i++)
        {
            var remainingTime = particles[i].remainingLifetime;

            //recycle
            if (time[i] < remainingTime)
            {
                if (CurvesOnUse[i]._segmentCount != resolution)
                    CurvesOnUse[i] = BezierCurve.FromToAvoidingObstacles(a, b, obstacles, resolution);
                else
                    CurvesOnUse[i].MakeFromToAvoidingObstacles(a, b, obstacles);
            }

            time[i] = particles[i].remainingLifetime;
            particles[i].position = CurvesOnUse[i].GetPos(1 - time[i] / _lifeTime);
        }
    }
}
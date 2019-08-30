using System.Collections;
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
        _lifeTime = _pSystem.main.startLifetime.constant;
    }

    public void Pop(Vector3 from, Vector3 to)
    {
        if (CurvesOnWait.Count != 0)
        {
            var curve = CurvesOnWait.Pop();
            BezierCurve.MakeFromToAvoidingObstacles(from, to, obstacles, curve);
            CurvesOnUse.Add(curve);
            return;
        }

        CurvesOnUse.Add(BezierCurve.FromToAvoidingObstacles(from, to, obstacles, resolution));
    }

    public void Recycle()
    {
        CurvesOnWait.Push(CurvesOnUse[0]);
        CurvesOnUse.RemoveAt(0);
    }


    public override void UpdateModule(ParticleSystem.Particle[] particles, int count)
    {
        while (count > CurvesOnUse.Count)
        {
            Pop(a, b);
        }

        while (count < CurvesOnUse.Count)
        {
            Recycle();
        }

        for (i = 0; i < count; i++)
        {
            particles[i].position = CurvesOnUse[i].GetPos(1 - particles[i].remainingLifetime / _lifeTime);
        }
    }
}
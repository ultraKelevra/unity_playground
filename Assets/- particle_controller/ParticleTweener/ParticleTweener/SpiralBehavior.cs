using System;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(ParticleTweenerUtility))]
public class SpiralBehavior : ParticleTweenerModule
{
    public override void InitializeModule(ParticleTweenerUtility particleTweener)
    {
    }

    private int i;

    public override void UpdateModule(ParticleSystem.Particle[] particles, int count)
    {
        for (i = 0; i < count; i++)
        {
            float t = i / (count - 1f);
            t = UnityEngine.Random.value;
            float z = t * t;
            float x = Mathf.Cos(i) * (1-z);
            float y = Mathf.Sin(i) * (1-z);

            particles[i].position = new Vector3(x, y, z);
        }
    }
}
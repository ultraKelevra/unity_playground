using UnityEngine;

public class ParticleDirectionTweener : ParticleTweenerModule
{
    private Vector3[] _previousPositions;

    public override void InitializeModule(ParticleTweenerUtility particleTweener)
    {
        _previousPositions = new Vector3[particleTweener.ParticleMaxCount];
    }

    private int i;

    public override void UpdateModule(ParticleSystem.Particle[] particles, int count)
    {
        for (i = 0; i < count; i++)
        {
            var forward = particles[i].position - _previousPositions[i];
            particles[i].rotation3D = Quaternion.LookRotation(forward, Vector3.up).eulerAngles;
            _previousPositions[i] = particles[i].position;
        }
    }
}
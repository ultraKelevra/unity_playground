using UnityEngine;

[RequireComponent(typeof(ParticleTweenerUtility))]

public abstract class ParticleTweenerModule : MonoBehaviour
{
    public abstract void InitializeModule(ParticleTweenerUtility particleTweener);
    public abstract void UpdateModule(ParticleSystem.Particle[] particles, int count);
}

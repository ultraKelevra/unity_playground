using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[ExecuteInEditMode]
public class ParticleTweenerUtility : MonoBehaviour
{
    public ParticleSystem.Particle[] particles
    {
        get { return _particles; }
    }

    public int ParticleCount
    {
        get { return _particleCount; }
    }

    public int ParticleMaxCount
    {
        get { return _particleMaxCount; }
    }

    private ParticleTweenerModule[] _modules;
    private ParticleSystem.Particle[] _particles;
    private int _particleMaxCount;
    private int _particleCount;
    private ParticleSystem _particleSystem;

    //aux
    private int i;

    // Start is called before the first frame update
    void OnEnable()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleMaxCount = _particleSystem.main.maxParticles;
        _particles = new ParticleSystem.Particle[_particleMaxCount];
        _modules = GetComponents<ParticleTweenerModule>();

        foreach (var module in _modules)
            module.InitializeModule(this);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _particleCount = _particleSystem.GetParticles(_particles);

        foreach (var module in _modules)
            module.UpdateModule(_particles, _particleCount);

        _particleSystem.SetParticles(_particles, _particleCount);
    }
}
using UnityEngine;

/// <summary>
/// 🐉
/// </summary>
public class BoomProjectileParticle : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke(nameof( ParticleStopped), 3);
    }

    void ParticleStopped()
    {
        SimplePool.Instance.Despawn(gameObject);
    }
}

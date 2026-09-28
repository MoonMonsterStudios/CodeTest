using NaughtyAttributes;
using UnityEngine;

namespace MoonMonster.Codetest
{
    public class ShellExplosion : MonoBehaviour
    {
        [SerializeField] private LayerMask _tankMask;
        [SerializeField, Required] private ParticleSystem _explosionParticles;
        [SerializeField, Required] AudioSource _explosionAudio;
        [SerializeField] private float _maxDamage = 100f;
        [SerializeField] private float _explosionForce = 1000f;
        [SerializeField] private float _maxLifeTime = 2f;
        [SerializeField] private float _explosionRadius = 5f;
        
        private void Start()
        {
            Destroy(gameObject, _maxLifeTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius, _tankMask);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (!colliders[i].TryGetComponent(out Rigidbody targetRigidbody))
                    continue;

                targetRigidbody.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);

                if (!targetRigidbody.TryGetComponent(out TankHealth targetHealth))
                    continue;

                float damage = CalculateDamage(targetRigidbody.position);
                targetHealth.TakeDamage(damage);
            }

            _explosionParticles.transform.parent = null;
            _explosionParticles.Play();
            _explosionAudio.Play();

            ParticleSystem.MainModule mainModule = _explosionParticles.main;
            Destroy(_explosionParticles.gameObject, mainModule.duration);

            Destroy(gameObject);
        }


        private float CalculateDamage(Vector3 targetPosition)
        {
            Vector3 explosionToTarget = targetPosition - transform.position;
            float explosionDistance = explosionToTarget.magnitude;
            float relativeDistance = (_explosionRadius - explosionDistance) / _explosionRadius;
            float damage = relativeDistance * _maxDamage;
            damage = Mathf.Max(0f, damage);

            return damage;
        }
    }
}
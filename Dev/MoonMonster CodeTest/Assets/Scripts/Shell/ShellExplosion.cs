using UnityEngine;

namespace MoonMonster.Codetest
{
    public class ShellExplosion : MonoBehaviour
    {
        public LayerMask TankMask;
        public ParticleSystem ExplosionParticles;
        public AudioSource ExplosionAudio;
        public float MaxDamage = 100f;
        public float ExplosionForce = 1000f;
        public float MaxLifeTime = 2f;
        public float ExplosionRadius = 5f;
        
        private void Start()
        {
            Destroy(gameObject, MaxLifeTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius, TankMask);

            for (int i = 0; i < colliders.Length; i++)
            {
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

                if (!targetRigidbody)
                    continue;

                targetRigidbody.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);

                TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

                if (!targetHealth)
                    continue;

                float damage = CalculateDamage(targetRigidbody.position);

                targetHealth.TakeDamage(damage);
            }

            ExplosionParticles.transform.parent = null;

            ExplosionParticles.Play();

            ExplosionAudio.Play();

            ParticleSystem.MainModule mainModule = ExplosionParticles.main;
            Destroy(ExplosionParticles.gameObject, mainModule.duration);

            Destroy(gameObject);
        }


        private float CalculateDamage(Vector3 targetPosition)
        {
            Vector3 explosionToTarget = targetPosition - transform.position;

            float explosionDistance = explosionToTarget.magnitude;

            float relativeDistance = (ExplosionRadius - explosionDistance) / ExplosionRadius;

            float damage = relativeDistance * MaxDamage;

            damage = Mathf.Max(0f, damage);

            return damage;
        }
    }
}
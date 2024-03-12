using System;
using UnityEditor.Rendering;
using UnityEngine;

namespace Complete
{
    public class ShellExplosion : MonoBehaviour
    {
        public LayerMask m_TankMask; // Used to filter what the explosion affects, this should be set to "Players".
        public ParticleSystem m_ExplosionParticles; // Reference to the particles that will play on explosion.
        public AudioSource m_ExplosionAudio; // Reference to the audio that will play on explosion.
        public float m_MaxDamage = 100f; // The amount of damage done if the explosion is centred on a tank.
        public float m_ExplosionForce = 1000f; // The amount of force added to a tank at the centre of the explosion.
        public float m_MaxLifeTime = 2f; // The time in seconds before the shell is removed.

        public float
            m_ExplosionRadius = 5f; // The maximum distance away from the explosion tanks can be and are still affected.


        private void Start()
        {
            // If it isn't destroyed by then, destroy the shell after it's lifetime.
            Destroy(gameObject, m_MaxLifeTime);
            m_ExplosionParticles.gameObject.SetActive(false);
        }


        private void OnTriggerEnter(Collider other)
        {
            // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

            // Go through all the colliders...
            for (int i = 0; i < colliders.Length; i++)
            {
                // ... and find their rigidbody.
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

                // If they don't have a rigidbody, go on to the next collider.
                if (!targetRigidbody)
                    continue;

                Vector3 forcePos = new Vector3(transform.position.x, 0, transform.position.z);
                
                // Add an explosion force.
                targetRigidbody.AddExplosionForce(m_ExplosionForce, forcePos , m_ExplosionRadius);

                // Find the TankHealth script associated with the rigidbody.
                TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

                // If there is no TankHealth script attached to the gameobject, go on to the next collider.
                if (!targetHealth)
                    continue;

                // Calculate the amount of damage the target should take based on it's distance from the shell.
                float damage = CalculateDamage(targetRigidbody.position);

                // Deal this damage to the tank.
                targetHealth.TakeDamage(damage);
            }

            // Unparent the particles from the shell.
            m_ExplosionParticles.transform.parent = null;

            // Play the particle system.
            m_ExplosionParticles.gameObject.SetActive(true);
            m_ExplosionParticles.Play();

            // Play the explosion sound effect.
            m_ExplosionAudio.Play();

            // Once the particles have finished, destroy the gameobject they are on.
            ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
            Destroy(m_ExplosionParticles.gameObject, mainModule.duration);

            // Destroy the shell.
            Destroy(gameObject);
        }


        private float CalculateDamage(Vector3 targetPosition) //üzerinden yediği damage gözükecek.
        {
            float damage = 0;
            //2 , 3, 4

            // Create a vector from the shell to the target.
            Vector3 explosionToTarget = targetPosition - transform.position;

            // Calculate the distance from the shell to the target.
            float explosionDistance = explosionToTarget.magnitude;

            if (explosionDistance <= 2.25f)
            {
                damage = m_MaxDamage;
            }
            else if (explosionDistance is > 2.25f and <= 3)
            {
                damage = m_MaxDamage * .4f;
            }
            else
            {
                damage = m_MaxDamage * .1f;
            }

            return damage;
        }
    }
}
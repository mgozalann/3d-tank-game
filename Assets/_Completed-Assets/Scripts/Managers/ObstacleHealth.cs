using System;
using UnityEngine;

namespace Complete
{
    public class ObstacleHealth : MonoBehaviour
    {
        private bool _dead; // Has the tank been reduced beyond zero health yet?

        [SerializeField] private float _maxHealth;
        [SerializeField] private float _maxDamage;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Shell"))
            {
                _maxHealth -= CalculateDamage(other.transform.position);
                if (_maxHealth <= 0)
                {
                    gameObject.SetActive(false);
                }
            }
        }

        private float CalculateDamage(Vector3 targetPosition) //üzerinden yediği damage gözükecek.
        {
            float damage = 0;
            //2 , 3, 4

            // Create a vector from the shell to the target.
            Vector3 explosionToTarget = targetPosition - transform.position;

            // Calculate the distance from the shell to the target.
            float explosionDistance = explosionToTarget.magnitude;

            if (explosionDistance <= 5)
            {
                damage = _maxDamage;
            }
            else if (explosionDistance is > 5 and <= 7.5f)
            {
                damage = _maxDamage * .4f;
            }
            else
            {
                damage = _maxDamage * .1f;
            }

            return damage;
        }
    }
}
using UnityEngine;

namespace Complete
{
    public class TankTurretControl : MonoBehaviour
    {
        [SerializeField] private GameObject _turret;

        private void OnEnable()
        {
            if (gameObject.tag == "Player")
            {
                GameManager.Current.OnTargetFocused += OnTargetFocused;
            }
            else
            {
                GameManager.Current.onEnemyFired += OnTargetFocused;
            }
        }

        private void OnDisable()
        {
            if (gameObject.tag == "Player")
            {
                GameManager.Current.OnTargetFocused -= OnTargetFocused;
            }
            else
            {
                GameManager.Current.onEnemyFired -= OnTargetFocused;
            }
        }

        
        private void OnTargetFocused(Vector3 obj)
        {
            Vector3 directionToTarget = obj - transform.position;
            directionToTarget.y = 0f; // Yönü sadece y ekseni üzerinden hesapla

            // Yeni rotasyonu bul
            Quaternion newRotation = Quaternion.LookRotation(directionToTarget);

            // Objeyi yeni rotasyona dönüştür
            _turret.transform.rotation = newRotation;
        }
    }
}
using UnityEngine;

namespace Complete
{
    public class TankShooting : MonoBehaviour
    {
        public int m_PlayerNumber = 1;
        public Rigidbody m_Shell; // Prefab of the shell.
        public Transform m_FireTransform; // A child of the tank where the shells are spawned.

        public AudioSource
            m_ShootingAudio; // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.

        public AudioClip m_FireClip; // Audio that plays when each shot is fired.

        [SerializeField] private GameObject _turret;
        [SerializeField] private float _rotation;

        private void OnEnable()
        {
            if (gameObject.tag == "Player")
            {
                GameManager.Current.OnFire += OnFire;
            }
            else
            {
                GameManager.Current.onEnemyFired += EnemyFire;
            }
        }

        private void OnDisable()
        {
            GameManager.Current.OnFire -= OnFire;
            GameManager.Current.onEnemyFired -= EnemyFire;
        }

        private void EnemyFire(Vector3 obj)
        {
            Vector3 directionToTarget = obj - transform.position;
            float distanceToTarget = Mathf.Abs(Vector3.Distance(obj, transform.position));
            Quaternion newRotation = Quaternion.LookRotation(directionToTarget);
            _turret.transform.rotation = newRotation;
            _turret.transform.Rotate(Vector3.right * _rotation);
            var shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
            float power = Mathf.Sqrt((distanceToTarget * 9.81f) / .5f) - 1;

            shellInstance.velocity = shellInstance.transform.forward * power;

            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();
        }

        private void OnFire()
        {
            Vector3 directionToTarget = GameManager.Current.RandomBulletDropPosition - transform.position;
            float distanceToTarget =
                Mathf.Abs(Vector3.Distance(GameManager.Current.RandomBulletDropPosition, transform.position));
            Quaternion newRotation = Quaternion.LookRotation(directionToTarget);
            _turret.transform.rotation = newRotation;
            _turret.transform.Rotate(Vector3.right * _rotation);
            var shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
            float power = Mathf.Sqrt((distanceToTarget * 9.81f) / .5f) - 1;

            shellInstance.velocity = shellInstance.transform.forward * power;

            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();
        }
    }
}
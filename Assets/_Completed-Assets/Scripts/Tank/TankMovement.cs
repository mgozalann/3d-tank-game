using UnityEngine;

namespace Complete
{
    public class TankMovement : MonoBehaviour
    {
        public int m_PlayerNumber = 1;
        public int RotationSpeed;
        public float m_Speed = 12f; // How fast the tank moves forward and back.

        public AudioSource m_MovementAudio; // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.

        public AudioClip m_EngineIdling; // Audio to play when the tank isn't moving.
        public AudioClip m_EngineDriving; // Audio to play when the tank is moving.

        public float m_PitchRange = 0.2f; // The amount by which the pitch of the engine noises can vary.

        private float m_OriginalPitch; // The pitch of the audio source at the start of the scene.
        private ParticleSystem[] m_particleSystems; // References to all the particles systems used by the Tanks

        private Rigidbody _rigidbody;
        private FixedJoystick _joystick;
        private TankDataSet _tankDataSet;

        private int _currentWaypointIndex = 0;

        private void OnEnable()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _tankDataSet = GetComponent<TankDataSet>();

            CheckIsEnemy();

            _joystick = FindObjectOfType<FixedJoystick>();
            _rigidbody.isKinematic = false;

            m_particleSystems = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < m_particleSystems.Length; ++i)
            {
                m_particleSystems[i].Play();
            }
        }

        private void CheckIsEnemy()
        {
            _tankDataSet.Tag = m_PlayerNumber != 1 ? "Enemy" : "Player";
            gameObject.tag = _tankDataSet.Tag;
        }

        private void OnDisable()
        {
            _rigidbody.isKinematic = true;
            for (int i = 0; i < m_particleSystems.Length; ++i)
            {
                m_particleSystems[i].Stop();
            }
        }

        private void Start()
        {
            m_OriginalPitch = m_MovementAudio.pitch;
        }

        private void Update()
        {
            EngineAudio();
        }


        private void EngineAudio()
        {
            // If there is no input (the tank is stationary)...
            if (Mathf.Abs(_rigidbody.velocity.magnitude) < 0.1f)
            {
                // ... and if the audio source is currently playing the driving clip...
                if (m_MovementAudio.clip == m_EngineDriving)
                {
                    // ... change the clip to idling and play it.
                    m_MovementAudio.clip = m_EngineIdling;
                    m_MovementAudio.pitch =
                        Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play();
                }
            }
            else
            {
                // Otherwise if the tank is moving and if the idling clip is currently playing...
                if (m_MovementAudio.clip == m_EngineIdling)
                {
                    // ... change the clip to driving and play.
                    m_MovementAudio.clip = m_EngineDriving;
                    m_MovementAudio.pitch =
                        Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                    m_MovementAudio.Play();
                }
            }
        }


        private void FixedUpdate()
        {
            if (_tankDataSet.Tag == "Player")
            {
                if (_joystick.Horizontal == 0 && _joystick.Vertical == 0)
                {
                    _tankDataSet.Idle = true;
                    _rigidbody.velocity = Vector3.zero;
                    
                }
                else
                {
                    _tankDataSet.Idle = false;
                }
            }
            else
            {
                if (_rigidbody.velocity.magnitude < 0.5f)
                {
                    _tankDataSet.Idle = true;
                }
                else
                {
                    _tankDataSet.Idle = false;
                }
            }

            if (!_tankDataSet.CanMove) return;
            Move();
            Turn();
        }


        private void Move()
        {
            if (_tankDataSet.Tag == "Player")
            {
                _rigidbody.velocity = new Vector3(_joystick.Horizontal * m_Speed, 0, _joystick.Vertical * m_Speed) *
                                      Time.deltaTime;
                Debug.Log(_rigidbody.velocity);
                Debug.Log(_joystick.Horizontal);
                Debug.Log(_joystick.Vertical);
            }
            else
            {
                if (Vector3.Distance(transform.position,
                        GameManager.Current.WayPoints[_currentWaypointIndex].position) < 0.1f)
                {
                    _rigidbody.velocity = Vector3.zero;

                    if (GameManager.Current.IsEnemyFocused) //box colliderına temas ediyorsa yürümeye başla
                    {
                        _currentWaypointIndex++;
                    }

                    // Waypointlerin sonuna geldiysek, ilk waypoint'e geri dön
                    if (_currentWaypointIndex >= GameManager.Current.WayPoints.Length)
                    {
                        _currentWaypointIndex = 0;
                    }
                }
                else
                {
                    Vector3 direction = GameManager.Current.WayPoints[_currentWaypointIndex].position -
                                        transform.position;

                    Vector3 velocity = direction.normalized * (m_Speed * Time.deltaTime);
                    velocity.y = 0;
                    _rigidbody.velocity = velocity;
                }
            }
        }


        private void Turn()
        {
            if (_tankDataSet.Tag == "Player")
            {
                if (_joystick.Horizontal != 0 || _joystick.Vertical != 0)
                {
                    Vector3 direction = new Vector3(_joystick.Horizontal, 0f, _joystick.Vertical);
                    Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime * RotationSpeed);
                }
            }
            else
            {
                Vector3 direction = GameManager.Current.WayPoints[_currentWaypointIndex].position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction);
                _rigidbody.MoveRotation(Quaternion.Lerp(_rigidbody.rotation, rotation, Time.deltaTime * RotationSpeed));
            }
        }
    }
}
using System;
using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Complete
{
    public class TankHealth : MonoBehaviour
    {
        public GameObject m_ExplosionPrefab;

        private AudioSource _explosionAudio; 
        private ParticleSystem _explosionParticles; 

        private bool _dead; 

        private TankDataSet _tankDataSet;
        [SerializeField] private TextMeshProUGUI _damageText;
        private void Awake()
        {
            _explosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();

            _explosionAudio = _explosionParticles.GetComponent<AudioSource>();

            _explosionParticles.gameObject.SetActive(false);
        }


        private void OnEnable()
        {
            _tankDataSet = GetComponent<TankDataSet>();
            _dead = false;
            _damageText.gameObject.SetActive(false);
        }



        public void TakeDamage(float amount)
        {
            // Reduce current health by the amount of damage done.
            _tankDataSet.CurrentHealth -= amount;

            GameManager.Current.HealthChanged(_tankDataSet);

            _damageText.text = (-amount).ToString();
            _damageText.gameObject.SetActive(true);
            _damageText.transform.DOLocalMoveY(_damageText.transform.position.y + 3, 1f).OnComplete(() => _damageText.gameObject.SetActive(false));

            if (_tankDataSet.CurrentHealth <= 0f && !_dead)
            {
                OnDeath();
            }
        }

        private void OnDeath()
        {
            _dead = true;

            _explosionParticles.transform.position = transform.position;
            _explosionParticles.gameObject.SetActive(true);

            _explosionParticles.Play();

            _explosionAudio.Play();

            gameObject.SetActive(false);
        }
    }
}
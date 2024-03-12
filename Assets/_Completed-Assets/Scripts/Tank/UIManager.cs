using System;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Slider _playerHealth; // The image component of the slider.
        [SerializeField] private Slider _enemyHealth; // The image component of the slider.

        private void OnEnable()
        {
            GameManager.Current.OnHealthChanged += OnOnHealthChanged;
        }

        private void OnOnHealthChanged(TankDataSet tankDataSet)
        {
            if (tankDataSet.Tag == "Enemy")
            {
                _enemyHealth.value = tankDataSet.CurrentHealth / tankDataSet.StartingHealth;
            }
            else
            {
                _playerHealth.value = tankDataSet.CurrentHealth / tankDataSet.StartingHealth;
            }
        }

        private void OnDisable()
        {
            GameManager.Current.OnHealthChanged -= OnOnHealthChanged;
        }
    }
}
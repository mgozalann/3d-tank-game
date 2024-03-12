using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace Complete
{
    public class TankDataSet : MonoBehaviour
    {
        public string Tag;
        public float CurrentHealth;
        public float StartingHealth = 100f;
        public bool CanMove;
        
        public event Action<TankDataSet, bool, bool> onIsIdleChanged;

        
        [SerializeField] private bool _isIdle;
        public bool Idle
        {
            get => _isIdle;
            set
            {
                bool oldValue = _isIdle;
                bool isChanged = _isIdle != value;
                _isIdle = value;
                if (isChanged)
                {
                    onIsIdleChanged?.Invoke(this, oldValue, value);
                }
            }
        }
    }
}
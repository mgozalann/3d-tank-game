using System;
using UnityEngine;
using UnityEngine.UI;

namespace Complete
{
    public class FuelController : MonoBehaviour
    {
        [SerializeField] private float _maxFuel = 100f;
        [SerializeField] private float _fuelConsumptionRate = 1f;
        [SerializeField] private float _fuelGenerationRate = 1f;

        [SerializeField] private Slider _fuelGauge;

        private float _currentFuel;
        private float _totalDistanceMoved;
        private TankDataSet _tankDataSet;

        private bool _isFull;
        private bool _canMove;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _tankDataSet = GetComponent<TankDataSet>();
            _rigidbody = GetComponent<Rigidbody>();
            _currentFuel = _maxFuel;
            _isFull = true;
        }

        private void OnEnable()
        {
            _tankDataSet.onIsIdleChanged += OnIdleChanged;
        }

        private void OnIdleChanged(TankDataSet arg1, bool arg2, bool arg3)
        {
            _isFull = Math.Abs(_currentFuel - _maxFuel) < 0.01f;
            _tankDataSet.CanMove = _isFull;
            GameManager.Current.IsEnemyFocused = false;
        }

        private void OnDisable()
        {
            _tankDataSet.onIsIdleChanged += OnIdleChanged;
        }

        private void Update()
        {
            if (_tankDataSet.Idle)
            {
                if (_currentFuel < _maxFuel)
                {
                    if (_tankDataSet.Tag == "Enemy")
                    {
                        _currentFuel += _fuelGenerationRate * 2 * Time.deltaTime;
                    }
                    else
                    {
                        _currentFuel += _fuelGenerationRate * Time.deltaTime;
                    }

                    _fuelGauge.value = _currentFuel / _maxFuel;
                }
                else
                {
                    _currentFuel = _maxFuel;
                    _tankDataSet.CanMove = true;
                }
            }
            else
            {
                _currentFuel -= _rigidbody.velocity.magnitude * _fuelConsumptionRate * Time.deltaTime;

                _fuelGauge.value = _currentFuel / _maxFuel;
                if (_currentFuel <= 0)
                {
                    _currentFuel = 0;
                    _tankDataSet.CanMove = false;
                }
            }
        }
    }
}
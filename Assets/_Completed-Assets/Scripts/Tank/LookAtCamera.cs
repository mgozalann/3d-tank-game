using System;
using UnityEngine;

namespace Complete
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private float _multiplier;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            transform.forward = _camera.transform.forward * _multiplier;
            transform.position = _targetTransform.position + _offset;
        }
    }
}
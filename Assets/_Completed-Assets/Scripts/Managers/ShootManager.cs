using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Complete
{
    public class ShootManager : MonoBehaviour
    {
        [SerializeField] private GameObject _enemyBulletArea;
        [SerializeField] private Button _fireButton;
        [SerializeField] private Image _countDownImage;
        [SerializeField] private PointerDownUpBehaviour _pointerDownUpBehaviour;

        [SerializeField] private LayerMask _layerMask;

        [SerializeField] private float _scaleMultiplier1;
        [SerializeField] private float _scaleMultiplier2;
        [SerializeField] private float _scaleFactor;
        [SerializeField] private float _countdown;

        private bool _fired;
        private float _currentCount;

        private Collider _enemyBulletAreaCollider;
        private Transform _focusedObject;
        private Camera _mainCamera;
        private TankDataSet _playerTankDataSet;
        
        private Vector3 _hitPoint;

        private void Start()
        {
            _enemyBulletAreaCollider = _enemyBulletArea.GetComponent<Collider>();
            SetFocusedObjectNull();
            _mainCamera = Camera.main;

            _playerTankDataSet = GameObject.FindWithTag("Player").GetComponent<TankDataSet>();
        }

        private void OnEnable()
        {
            _fireButton.onClick.AddListener(Fire);
            _pointerDownUpBehaviour.onPointerUp+=OnPointerUpEvent;
        }

        private void OnDisable()
        {
            _fireButton.onClick.RemoveListener(Fire);
            _pointerDownUpBehaviour.onPointerUp-=OnPointerUpEvent;
        }
        
        private void OnPointerUpEvent(PointerEventData obj)
        {
            if (_focusedObject != null)
            {
                GameManager.Current.IsFocused = true;
                GameManager.Current.TargetFocused(_hitPoint);
                _enemyBulletAreaCollider.enabled = true;
            }
        }


        private void Fire()
        {
            float circleRadius = (_enemyBulletArea.transform.localScale.x - .25f) / 2;
            Vector3 circleCenter = _enemyBulletArea.transform.position;

            float randomAngle = Random.Range(0f, 2f * Mathf.PI);
            float randomRadius = Random.Range(0f, circleRadius);

            float randomX = circleCenter.x + randomRadius * Mathf.Cos(randomAngle);
            float randomY = circleCenter.y;
            float randomZ = circleCenter.z + randomRadius * Mathf.Sin(randomAngle);

            Vector3 randomPointInsideCircle = new Vector3(randomX, randomY, randomZ);

            GameManager.Current.RandomBulletDropPosition = randomPointInsideCircle;
            GameManager.Current.OnFireClick();

            SetFocusedObjectNull();
            _fired = true;
        }
        
        private void Update()
        {
            if (_fired)
            {
                _fireButton.interactable = false;

                _currentCount += Time.deltaTime;
                _countDownImage.fillAmount = _currentCount / _countdown;

                if (_currentCount >= _countdown)
                {
                    _fired = false;
                    _fireButton.interactable = true;
                    _currentCount = 0;
                }
            }

            if (_pointerDownUpBehaviour.IsPointerDown)
            {
                if (GameManager.Current.IsFocused) return;

                Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMask))
                {
                    if (!hit.collider.CompareTag("Ground")) return;

                    _hitPoint = hit.point;
                    _hitPoint.y = 0.05f;

                    _enemyBulletArea.transform.localScale = Vector3.one * _scaleFactor;
                    _enemyBulletArea.transform.position = _hitPoint + Vector3.up * 0.05f;
                    _enemyBulletArea.SetActive(true);

                    _focusedObject = hit.collider.transform;
                }
            }
            else
            {
                if (_focusedObject != null)
                {
                    BulletArea();
                }
            }

            if (!_playerTankDataSet.Idle && _focusedObject != null)
            {
                SetFocusedObjectNull();
            }
        }

        private void BulletArea()
        {
            //_bulletArea.transform.position = _focusedObject.position + Vector3.up * 0.05f;
            if (_enemyBulletArea.transform.localScale.x > 9f)
            {
                _enemyBulletArea.transform.localScale *= _scaleMultiplier1;
            }
            else if (_enemyBulletArea.transform.localScale.x is < 13f and >= 2.5f)
            {
                _enemyBulletArea.transform.localScale *= _scaleMultiplier2;
            }
            else
            {
                SetFocusedObjectNull();
            }
        }

        private void SetFocusedObjectNull()
        {
            _enemyBulletArea.SetActive(false);
            _enemyBulletAreaCollider.enabled = false;
            _focusedObject = null;
            GameManager.Current.IsFocused = false;
        }
    }
}
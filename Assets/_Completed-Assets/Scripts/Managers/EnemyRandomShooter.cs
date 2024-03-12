using UnityEngine;

namespace Complete
{
    public class EnemyRandomShooter : MonoBehaviour
    {
        [SerializeField] private GameObject _playerBulletArea;

        [SerializeField] private float _scaleMultiplier1;
        [SerializeField] private float _scaleMultiplier2;
        [SerializeField] private float _scaleFactor;

        public float _deployCD;
        public float _fireCD;
        public float _fireCounter;
        public float _deployCounter;
        
        public bool _fired;
        public bool _deploy;

        private TankDataSet _enemyTankDataSet;
        private GameObject _playerTank;

        private void Start()
        {
            _deployCD = Random.Range(6, 9);
            _playerTank = GameObject.FindWithTag("Player");
            _playerBulletArea.SetActive(false);
            _enemyTankDataSet = GameObject.FindWithTag("Enemy").GetComponent<TankDataSet>();
        }


        private void Fire()
        {

            float circleRadius = (_playerBulletArea.transform.localScale.x - .25f) / 2;
            Vector3 circleCenter = _playerBulletArea.transform.position;

            float randomAngle = Random.Range(0f, 2f * Mathf.PI);
            float randomRadius = Random.Range(0f, circleRadius);

            float randomX = circleCenter.x + randomRadius * Mathf.Cos(randomAngle);
            float randomY = circleCenter.y;
            float randomZ = circleCenter.z + randomRadius * Mathf.Sin(randomAngle);

            Vector3 randomPointInsideCircle = new Vector3(randomX, randomY, randomZ);

            GameManager.Current.OnEnemyFired(randomPointInsideCircle);

            _playerBulletArea.SetActive(false);
            _fired = true;
            _deploy = false;
            _enemyTankDataSet.CanMove = true;

        }

        private void Update()
        {
            if (_deploy)
            {
                if (!_fired)
                {
                    BulletArea();
                    _fireCounter += Time.deltaTime;

                    if (_fireCounter >= _fireCD)
                    {
                        Fire();
                        
                        _fireCounter = 0;
                        _deployCD = Random.Range(7, 10);
                    }
                }

            }
            else
            {
                _deployCounter += Time.deltaTime;

                if (_deployCounter >= _deployCD)
                {
                    _enemyTankDataSet.CanMove = false;
                    
                    _playerBulletArea.transform.localScale = Vector3.one * _scaleFactor;
                    _playerBulletArea.transform.position = _playerTank.transform.position + Vector3.up * 0.05f;
                    
                    _playerBulletArea.SetActive(true);
                    
                    _fireCD = Random.Range(2, 5);
                    _deployCounter = 0;
                    
                    _fired = false;
                    _deploy = true;
                }
            }
        }

        private void BulletArea()
        {
            if (_playerBulletArea.transform.localScale.x > 9f)
            {
                _playerBulletArea.transform.localScale *= _scaleMultiplier1;
            }
            else if (_playerBulletArea.transform.localScale.x is < 13f and >= 2.5f)
            {
                _playerBulletArea.transform.localScale *= _scaleMultiplier2;
            }
        }
    }
}
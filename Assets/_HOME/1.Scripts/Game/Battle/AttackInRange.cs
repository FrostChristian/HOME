using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOME.Game {

    public class AttackInRange : MonoBehaviour {

        [Header("Attack Attributes")]
        [SerializeField] private List<Transform> _projectileSpawnPoint = new List<Transform>();
        [SerializeField] private GameObject _procetilePrefab = default;
        [SerializeField] private GameObject _impactEffect = default;

        [SerializeField] private float _attackDamage = 1f;//dmg
        [SerializeField] private float _attackRate = 2f;
        [SerializeField] private float _maxAttackRange = 50f; //units cant shoot past this distance
        [SerializeField] private float _minAttackRange = 10f; //units cant shoot past this distance
        [SerializeField] private float _attackCooldown = 5f; // per seconds
        [SerializeField] private float _findNewTargetCooldown = 1; //find a new target after this time

        [Header("Rotation")]
        [SerializeField] private Transform _Rotation = default;
        [SerializeField] private Transform _Pivot = default;
        [SerializeField] private float _turnSpeed = 10f;

        [Header("Rocket Launcher")]

        [SerializeField] private float velocityOffset = 0.38f;
        public float CalcOffset1 = 1.51f;
        public float CalcOffset2 = 1.22f;


        [Header("Show Unity Info")]
        [SerializeField] private Transform _target = default; // what are we shooting at
        [SerializeField] private Entity _targetInfo = default; // what are we shooting at

        private PlayerSetupDefinition _player; //who is this player
        private float _currentFindTargetCooldown = 0f; //time passed since we found target and shot

        private GameManager _gameManager;
        [SerializeField] private bool rocketLauncher = default;


        void Start() {
            _gameManager = FindObjectOfType<GameManager>();
        }

        void Update() {
            if (_target) {//Debug
                DrawTargetLines();
            }

            if (_Rotation != null && _target != null && _Pivot != null) {//Rotation
                Rotation();
            }

            if (_currentFindTargetCooldown <= 0) {
                UpdateTarget();
                _currentFindTargetCooldown = 0 + _findNewTargetCooldown;
            }
            _currentFindTargetCooldown -= Time.deltaTime;

            if (_attackCooldown <= 0) { //can we shoot?
                Shoot();
                _attackCooldown = 1 / _attackRate; //add firerate
            }
            _attackCooldown -= Time.deltaTime; //count down
        }

        private void Rotation() {
            if (!rocketLauncher) {
                Vector3 targetDirection = _target.position - transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(targetDirection);
                Vector3 rotationRotation = Quaternion.Lerp(_Rotation.rotation, lookRotation, Time.deltaTime * _turnSpeed).eulerAngles;
                _Rotation.rotation = Quaternion.Euler(0f, rotationRotation.y, 0f); // set rotation of rotationOBJ
                _Pivot.rotation = Quaternion.Euler(rotationRotation.x, rotationRotation.y, 0f); // set rotation of PivotOBJ

            } else {
                if (_target == null) {
                    return;
                }

                Vector3 distanceDir = CalculateProjectile(CalcOffset1); // rocketlauncher
                _Pivot.rotation = Quaternion.LookRotation(distanceDir);
            }
        }

        private Vector3 CalculateProjectile(float time) {

            Vector3 targetDirection = _target.position - transform.position;
            Vector3 distanceXZ = targetDirection; // dist on XZ plane
            distanceXZ.y = 0f; // only plane

            float verticalDir = targetDirection.y;
            float horizontalDistance = distanceXZ.magnitude; // horizontal length of distanceXZ

            float velocityXZ = horizontalDistance / time; // calculate horizontal velocity
            float velocityY = verticalDir / time + velocityOffset * Mathf.Abs(Physics.gravity.y) * time;// calculate vertical velocity

            Vector3 target = distanceXZ.normalized; // return direction
            target *= velocityXZ;
            target.y = velocityY;

            return target;
        }

        void UpdateTarget() { // called after x time has passed
            if (_player == null) {
                _player = GetComponent<Player>().Info;
            }
            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;
            foreach (var player in _gameManager.activePlayers) { // look for the players units
                if (player == _player) {
                    continue; //skip rest of code
                }
                foreach (var unit in player.ActiveUnits) { //anything in attakRange to attack the players units?
                    float distToEnemy = Vector3.Distance(transform.position, unit.transform.position);
                    if (distToEnemy < shortestDistance) {
                        shortestDistance = distToEnemy;
                        nearestEnemy = unit;
                    }
                }
                if (nearestEnemy != null && shortestDistance <= _maxAttackRange && shortestDistance >= _minAttackRange) {
                    _target = nearestEnemy.transform;
                    _targetInfo = nearestEnemy.GetComponent<Entity>();
                } else {
                    _target = null;
                }
            }
        }

        void Shoot() {
            if (_target == null) {
                return;
            }

            _targetInfo.CurrHealth -= _attackDamage; // TODOdo this on hit

            GameObject go = Instantiate(_impactEffect, _target.transform.position, Quaternion.identity, GameManager.InstanceGraveParent());
            Destroy(go, 1f); //destroy go after 1 seconds

            foreach (var weapon in _projectileSpawnPoint) {
                if (rocketLauncher) {
                    StartCoroutine(LaunchRocket(weapon));
                } else {
                    GameObject projectileGO = Instantiate(_procetilePrefab, weapon.position, weapon.rotation, GameManager.InstanceGraveParent());
                    Projectile projectile = projectileGO.GetComponent<Projectile>();
                    if (projectile != null) {
                        projectile.SetTarget(_target);
                        projectile.SetDamage(_attackDamage);
                    }
                }
            }
        }

        IEnumerator LaunchRocket(Transform weapon) {
            float wait_time = Random.Range(0f, 1f);
            yield return new WaitForSeconds(wait_time);

            if (_target == null) {
                yield break; // stop if no target
            }

            GameObject obj = Instantiate(_procetilePrefab, weapon.position, weapon.rotation, GameManager.InstanceGraveParent());
            Rigidbody parojectile = obj.GetComponent<Rigidbody>(); // rigidbody for velocity cont
            Projectile projectiles = obj.GetComponent<Projectile>();

            parojectile.velocity = CalculateProjectile(CalcOffset2);
            if (projectiles != null) {
                projectiles.SetTarget(_target);
                projectiles.SetDamage(_attackDamage);
            }
        }

        // gizmos
        private void DrawTargetLines() {
            if (_target.GetComponent<Player>().Info.isAi) {
                Debug.DrawLine(transform.position, _target.transform.position, Color.green);

            }
            if (!_target.GetComponent<Player>().Info.isAi) {
                Debug.DrawLine(transform.position, _target.transform.position, Color.red);


            }
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _maxAttackRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _minAttackRange);
        }
    }
}
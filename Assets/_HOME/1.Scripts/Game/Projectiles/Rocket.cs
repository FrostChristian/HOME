using UnityEngine;

namespace HOME.Game {

    public class Rocket : Projectile {

        [SerializeField] private GameObject _rocketTrailprefab; // trail
        private GameObject _rocketTrail; //set trail prefab to this
        [SerializeField] private float _turnSpeed = 3f; // rocket lerp speed

        private void Awake() {
            _rocketTrail = Instantiate(_rocketTrailprefab); // instantiate trail
            _rocketTrail.transform.position = transform.position;  // set pos and rota to rocket
            _rocketTrail.transform.rotation = transform.rotation;
            Destroy(_rocketTrail, 2f); // set destroy timer to 2 seconds
        }

        public override void Update() {

            if (_rocketTrail != null) {
                _rocketTrail.transform.position = transform.position; // as long as the rocket is alive keep updating its trail posi
            }

            if (target == null) {        // no Target no Update     
                return;
            }

            Vector3 direction = target.position - transform.position; // get dir

            float distanceThisFrame = speed * Time.deltaTime; // get distance moved this frame
            if (direction.magnitude <= distanceThisFrame) { // if the length of the magnitude is less then distanceThis frame? then we already hit the target!
                HitTarget(); // hit 
                Destroy(_rocketTrail); // if not destroyed already destroy now
                return;
            }

            Quaternion lookRotation = Quaternion.LookRotation(direction); // get rotation to target
            Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * _turnSpeed).eulerAngles; // lerp from initial rota to target rotation
            transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z); // apply rotation
        }
    }
}
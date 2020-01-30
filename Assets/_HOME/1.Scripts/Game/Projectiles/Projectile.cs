using UnityEngine;

namespace HOME.Game {

    public abstract class Projectile : MonoBehaviour {

        public GameObject impactEffectPrefab;
        [HideInInspector] public Transform target;
        [HideInInspector] public float speed = 10f; // default value
        [HideInInspector] public float _projectileDamage = 20f;// default value

        public void SetTarget(Transform target) {
            this.target = target;
        }

        public void SetDamage(float projectileDamage) {
            _projectileDamage = projectileDamage;
        }

        public virtual void Update() {
            if (target == null) {
                Destroy(gameObject);
                return;
            }
        }

        public void HitTarget() {
            GameObject effect = Instantiate(impactEffectPrefab, transform.position, transform.rotation, GameManager.InstanceGraveParent());
            Destroy(effect, 2f); // desroy impackt prefab
            Destroy(gameObject); // if we hit destroy rocket
        }
    }
}
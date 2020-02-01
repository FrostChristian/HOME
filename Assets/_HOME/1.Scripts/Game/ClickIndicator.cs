using UnityEngine;

namespace HOME.Game {
    // on right click on map
    public class ClickIndicator : MonoBehaviour {

        [SerializeField] private GameObject _ring1 = default;
        [SerializeField] private GameObject _ring2 = default;
        [SerializeField] private GameObject _ring3 = default;
        private float timer = 1f;

        public static ClickIndicator _instance;

        private void Awake() {
            Destroy(_ring1, timer);
            Destroy(_ring2, timer - 0.2f);
            Destroy(_ring3, timer - 0.4f);
            Destroy(gameObject, timer);
        }
    }
}

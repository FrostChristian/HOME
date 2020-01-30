using UnityEngine;

namespace HOME.Game {

    public class PlanetRandom : MonoBehaviour {

        void Awake() {
            transform.rotation = Random.rotation;
        }
    }
}

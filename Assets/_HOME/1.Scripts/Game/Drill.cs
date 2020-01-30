using System.Collections;
using UnityEngine;

namespace HOME.Game {

    public class Drill : MonoBehaviour {
        // handles the drill on the mine building

        public Transform drillGameObject;
        public float drillSpeed = .5f;
        public float drillRotation = 720f;
        public float range = 2f;

        private float _currDrillSpeed = 0f;
        private int n = -1;
        private Vector3 _startPosition;
        private Vector3 _rotation;

        void Start() {
            _currDrillSpeed = drillSpeed;
            _startPosition = drillGameObject.position;
        }

        void Update() {
            SetDir();
            drillGameObject.Translate(0, _currDrillSpeed * Time.deltaTime * n, 0);
            drillGameObject.Rotate(_rotation * Time.deltaTime);
        }

        private void SetDir() {
            if ((drillGameObject.position.y > (_startPosition.y + range))) {
                StartCoroutine(StartDrill(new Vector3(0f, drillRotation, 0f), 2f));
            }
            if ((drillGameObject.position.y < (_startPosition.y))) {
                StartCoroutine(StopDrill(Vector3.zero, 5f));
            }
        }

        IEnumerator StartDrill(Vector3 endRota, float duration) {
            drillGameObject.position = new Vector3(_startPosition.x, _startPosition.y + range, _startPosition.z);
            n = n * -1;
            _currDrillSpeed = 0f;
            float elapsedTime = 0;
            while (elapsedTime < duration) {
                _rotation = Vector3.Lerp(_rotation, endRota, (elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            _rotation = new Vector3(0f, drillRotation, 0f); // set rota
            _currDrillSpeed = 0.05f;
        }

        IEnumerator StopDrill(Vector3 endRota, float duration) {
            drillGameObject.position = new Vector3(_startPosition.x, _startPosition.y, _startPosition.z); // set drill pos
            _currDrillSpeed = 0f; // stop movement
            float elapsedTime = 0;
            while (elapsedTime < duration) {
                _rotation = Vector3.Lerp(_rotation, endRota, 0.05f);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            _rotation = Vector3.zero;
            n = n * -1; //referse direction
            _currDrillSpeed = 1f; //set movespeed
        }
    }
}

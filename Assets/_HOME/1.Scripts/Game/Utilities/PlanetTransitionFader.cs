using HOME.UI;
using System.Collections;
using UnityEngine;

namespace HOME.Game {

    public class PlanetTransitionFader : ScreenFader {

        private Transform _planetSelected;
        public Transform PlanetSelected { get => _planetSelected; set => _planetSelected = value; }

        [Header("Canvas Fade")]
        private float _lifetime = 1f; // how long will transition take
        [SerializeField] private float _delay = 0.3f; // how long still we start fading off or on
        public float Delay { get { return _delay; } }
        [SerializeField] private CanvasGroup _canvasToFade;

        [Header("Planet Movement")]
        public float speed = 10.0f;
        private bool moving;
        private float startTime;
        private float journeyLength;
        [Space]
        [SerializeField] private Transform _middleSpawnTransform;

        [Header("Camera Movement")]
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private float lookAtDamping = 4.0f;    // Look at (rotational) damping. Lower value = smoother
        private float distance = 0f;    // depending on damping the distance will increase at speed)
        private float _higntOffset = 0f;    // Height for chase mode camera
        private float followDamping = 0.3f;    // Follow (movement) damping. Lower value = smoother

        protected void Awake() {
            _middleSpawnTransform = GameObject.Find("PlanetSpawn2").transform;
            _mainCamera = Camera.main;
            _lifetime = Mathf.Clamp(_lifetime, FadeOnDuration + FadeOffDuration + _delay, 10f); // set lifetime minimum´, FadeOn + FadeOff + Delay
        }

        void Update() {
            if (!moving) {
                return;
            }
            MovingPlanet2();
            CameraLookAtPlanet();
        }

        private IEnumerator PlayRoutine() {
            yield return new WaitForSeconds(_delay);
            FadeUnselecetdPlanets();
            // start fading
            //movement
            journeyLength = Vector3.Distance(_planetSelected.position, _middleSpawnTransform.position);
            startTime = Time.time;
            moving = true;
            //+monvement
            yield return StartCoroutine(FadeCanvas(PlanetSelectMenu.CanvasGroup_Static, _solidAlpha, _clearAlpha, FadeOffDuration));
            float onTime = _lifetime - (FadeOffDuration + _delay);
            //yield return new WaitForSeconds(onTime);
            Object.Destroy(gameObject, FadeOffDuration);
        }

        private void CameraLookAtPlanet() {
            // Smooth lookat interpolation
            Quaternion _lookAt = _middleSpawnTransform.rotation;
            _mainCamera.transform.rotation = Quaternion.Lerp(_mainCamera.transform.rotation, _lookAt, Time.deltaTime * lookAtDamping);
            // Smooth follow interpolation
            //_mainCamera.transform.position = Vector3.Lerp(_mainCamera.transform.position, _middleSpawnTransform.position - _middleSpawnTransform.forward * distance + _middleSpawnTransform.up * _higntOffset, Time.deltaTime * followDamping * 10);
        }

        private void MovingPlanet() {
            float step = speed * Time.deltaTime; // calculate distance to move
            _planetSelected.position = Vector3.MoveTowards(_planetSelected.position, _middleSpawnTransform.position, step);
            if (Vector3.Distance(_planetSelected.position, _middleSpawnTransform.position) < 0.001f) {// Check if the position of the cube and sphere are approximately equal.  
            }
        }

        private void MovingPlanet2() {
            float distCovered = (Time.time - startTime) * speed;  // Distance moved equals elapsed time times speed..
            float fractionOfJourney = distCovered / journeyLength;    // Fraction of journey completed equals current distance divided by total distance.
            _planetSelected.position = Vector3.Lerp(_planetSelected.position, _middleSpawnTransform.position, fractionOfJourney);  // Set our position as a fraction of the distance between the markers.
        }

        private void FadeUnselecetdPlanets() {
            GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet"); // find all planets in scene
            if (planets != null) {
                foreach (var planet in planets) { //look for selected planet
                    if (planet.transform == _planetSelected) {
                        Debug.Log("planet" + planet);
                    } else {
                        Destroy(planet);// destroy everything else
                    }
                }
            }
        }

        public void Play(Transform planetSelected) {
            _planetSelected = planetSelected;
            StartCoroutine(PlayRoutine());
        }

        public static void PlayTransition(PlanetTransitionFader transitionPrefab, Transform planetSelected) {
            if (transitionPrefab != null && planetSelected != null) {
                PlanetTransitionFader instance = Instantiate(transitionPrefab, Vector3.zero, Quaternion.identity);
                instance.Play(planetSelected);
            }
        }
    }
}

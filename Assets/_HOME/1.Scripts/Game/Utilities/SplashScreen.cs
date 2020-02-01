using HOME.UI;
using System.Collections;
using UnityEngine;

namespace HOME.Game {

    [RequireComponent(typeof(ScreenFader))] // add screenfader
    public class SplashScreen : MonoBehaviour {

        [SerializeField] private ScreenFader _screenFader = default;
        [SerializeField] private float delay = 1f;

        private void Awake() {
            _screenFader = GetComponent<ScreenFader>(); //get fader     
        }

        private void Start() {
            _screenFader.FadeOn(); // fade on
        }

        public void FadeAndLoad() {
            StartCoroutine(FadeAndLoadRoutine());
        }

        private IEnumerator FadeAndLoadRoutine() {
            yield return new WaitForSeconds(delay);
            _screenFader.FadeOff();
            LevelLoader.LoadMainMenuLevel();

            yield return new WaitForSeconds(_screenFader.FadeOnDuration);
            Destroy(gameObject);
        }
    }
}

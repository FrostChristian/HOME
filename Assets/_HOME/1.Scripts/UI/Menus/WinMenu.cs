using HOME.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HOME.UI {

    public class WinMenu : Menu<WinMenu> {

        [SerializeField] private float _playDelay;
        [SerializeField] private TransitionFader _startTransitionPrefab;

        public void OnNextLevelPressed() {
            StartCoroutine(StartGameRoutine(3)); // load up the appropriate level after transition
        }

        public void OnRestartPressed() {
            DataManager.Instance.Load();
            StartCoroutine(StartRestartGameRoutine());
        }

        public void OnLoadPressed() {
            LoadMenu.Open();
        }

        public void OnMainMenuPressed() {
            LevelLoader.LoadMainMenuLevel();
            MainMenu.Open();
        }

        public override void OnBackPressed() {
            base.OnBackPressed();
        }
        IEnumerator StartRestartGameRoutine() {
            TransitionFader.PlayTransition(_startTransitionPrefab);
            yield return new WaitForSecondsRealtime(_playDelay);
            LevelLoader.LoadLevel(2); // only Index numbers !!!!!
            InGameMenu.Open();
        }

        IEnumerator StartGameRoutine(int sceneIndex) {
            TransitionFader.PlayTransition(_startTransitionPrefab);
            yield return new WaitForSeconds(_playDelay);
            LevelLoader.LoadLevel(sceneIndex);
            PlanetSelectMenu.Open();
        }
    }
}
using HOME.Data;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HOME.UI {

    public class LoseMenu : Menu<LoseMenu> {

        [Header("Transiton")]
        [SerializeField] private TransitionFader _startTransitionPrefab = default;
        [SerializeField] private Text _restartConfirmText = default;
        [SerializeField] private GameObject _restartConfirm = default;

        public override void OnBackPressed() {
            base.OnBackPressed();
        }

        public void OnLoadPressed() {
            LoadMenu.Open();
            Debug.Log("LoseMenu OnLoadPressed(): LoadMenu.Open()");
        }

        public void OnMainMenuPressed() {
            Time.timeScale = 1;
            LevelLoader.LoadMainMenuLevel();
            MainMenu.Open();
            Debug.Log("LoseMenu OnMainMenuPressed(): MainMenu.Open()");
        }

        public void OnRestartPressed() {
            Time.timeScale = 0;
            StartCoroutine(PrepareLoad());
        DataManager.Instance.Load();
        }

        IEnumerator PrepareLoad() {
            _restartConfirmText.text = "Restart";
            _restartConfirm.SetActive(true);
            yield return new WaitForSecondsRealtime(0.3f);
            _restartConfirmText.text += ".";
            yield return new WaitForSecondsRealtime(0.3f);
            _restartConfirmText.text += ".";
            yield return new WaitForSecondsRealtime(0.3f);
            _restartConfirmText.text += ".";
            StartCoroutine(StartGameRoutine()); // load the level
            yield return new WaitForSecondsRealtime(0.1f);
            _restartConfirm.SetActive(false);
        }

        IEnumerator StartGameRoutine() {
            TransitionFader.PlayTransition(_startTransitionPrefab);
            yield return new WaitForSecondsRealtime(1f);
            LevelLoader.LoadLevel(2); // only Index numbers !!!!!
            InGameMenu.Open();
        }
    }
}

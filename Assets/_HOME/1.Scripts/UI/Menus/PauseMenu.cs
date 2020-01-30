using UnityEngine;
using UnityEngine.SceneManagement;

namespace HOME.UI {

    public class PauseMenu : Menu<PauseMenu> {

        public void OnResumePressed() {
            Time.timeScale = 1;
            base.OnBackPressed();
        }

        public void OnRestartPressed() {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            base.OnBackPressed();
        }

         public void OnMainMenuPressed() {
            Time.timeScale = 1;
            LevelLoader.LoadMainMenuLevel();
            MainMenu.Open();
         }

        public void OnLoadPressed() {
            LoadMenu.Open();
            LoadMenu._init = true;
        }

        public void OnSavePressed() {
            SaveMenu.Open();
            SaveMenu._initS = true;
        }

        public void OnOptionsPressed() {
            OptionsMenu.Open();
        }

        public override void OnBackPressed() {
            Application.Quit();
        }
    }
}
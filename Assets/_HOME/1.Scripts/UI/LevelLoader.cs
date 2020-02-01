using HOME.Game;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HOME.UI
{

    public class LevelLoader : MonoBehaviour
    {

        private static int mainMenuIndex = 1; // KEEP MAIN MENU @ 1!!

        [SerializeField] private LoadingBar _loadProgressBarPrefab = default;
        private LoadingBar _loadProgressBar;

        public static void LoadLevel(string levelName)
        {
            if (Application.CanStreamedLevelBeLoaded(levelName))
            {
                SceneManager.LoadScene(levelName);
            }
            else
            {
                Debug.LogWarning("LEVELLOADER LOADLEVEL ERROR: invalid scene name specified");
            }
        }

        public static void LoadLevel(int levelIndex)
        {
            if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
            {
                if (levelIndex == mainMenuIndex)
                {
                    MainMenu.Open();
                }
                LevelLoader levelLoader = FindObjectOfType<LevelLoader>();  // for async loading, we need a specific object

                if (levelLoader != null)
                { // load with progress bar if we find a LevelLoad object
                    levelLoader.LoadLevelAsync(levelIndex);
                    Debug.Log("Load Level " + levelIndex);
                }
                else
                {  // otherwise load immediately
                    SceneManager.LoadScene(levelIndex);
                }
            }
            else
            {
                Debug.Log("LEVELLOADER LoadLevel Error: invalid scene specified!");
            }
        }

        private void LoadLevelAsync(int levelIndex)
        { // start the coroutine to load asynchronously and generate a progress bar prefab
            if (_loadProgressBarPrefab != null)
            {
                _loadProgressBar = Instantiate(_loadProgressBarPrefab, Vector3.zero, Quaternion.identity);
                _loadProgressBar.InitSlider();
            }
            StartCoroutine(LoadLevelAsyncRoutine(levelIndex));
        }

        private IEnumerator LoadLevelAsyncRoutine(int levelIndex)
        { // load a level asynchronously and update the load progress bar
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelIndex);
            asyncLoad.allowSceneActivation = false;

            if (_loadProgressBar != null)
            {
                while (_loadProgressBar.sliderValue < 0.9f)
                {
                    _loadProgressBar.UpdateProgress(asyncLoad.progress);
                    yield return null;
                }
            }
            yield return null;
            asyncLoad.allowSceneActivation = true;
        }

        public static void ReloadLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public static void LoadNextLevel()
        {
            int nextLevelIndex = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
            nextLevelIndex = Mathf.Clamp(nextLevelIndex, mainMenuIndex, nextLevelIndex);
            LoadLevel(nextLevelIndex);
        }

        public static void LoadMainMenuLevel()
        {
            LoadLevel(mainMenuIndex);
        }
    }
}

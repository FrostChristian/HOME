using HOME.Game;
using HOME.Data;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HOME.UI {
    public class GameSetupMenu : Menu<GameSetupMenu> {

        [SerializeField] private DifficultyManager _difficultyManager = default;
        [SerializeField] private Image _previewImage = default;
        [SerializeField] private Text _nameText = default;
        [SerializeField] private Text _descriptionText = default;
        [SerializeField] private Text _infoText = default;
        [SerializeField] private GameObject _lockIcon = default;
        [SerializeField] private float _playDelay = 0.5f; // delay before we play the game
        [SerializeField] private TransitionFader _startTransitionPrefab = default; // reference to transition prefab

        private string _defaultInfo;

        protected override void Awake() {
            base.Awake();
            _difficultyManager = GetComponent<DifficultyManager>();
        }

        private void OnEnable() {
            UpdateUI();
        }

        public void Select(int levelIndex) {  // load up a level by index
            _difficultyManager.SetIndex(levelIndex);
        }

        public void UpdateUI() {
            GameDifficultyDefinition currentDifficulty = _difficultyManager?.GetCurrentDifficulty();
            if (currentDifficulty == null) {
                return;
            }
            _previewImage.sprite = currentDifficulty?.Image;
            _nameText.text = currentDifficulty.Name;
            _descriptionText.text = currentDifficulty.Description;
            _infoText.text = string.Empty;
            _lockIcon.gameObject.SetActive(false);   // check lock status, currently defaulting to off

        }

        public void OnNextPressed() {
            _difficultyManager.IncrementIndex();
            UpdateUI();
        }

        public override void OnBackPressed() {
            base.OnBackPressed();
            DataManager.Instance.DeleteData(); // delete data 
            LoadMenu._init = true;
            LoadMenu.Open();
        }

        public void OnPreviousPressed() {
            _difficultyManager.DecrementIndex();
            UpdateUI();
        }

        public void OnPlayerNameValueChanged(string name) {
            if (DataManager.Instance != null) {
                DataManager.Instance.PlayerName = name;
                Debug.Log(DataManager.Instance.PlayerName);
            }
        }
        /*
        public void OnPlayerNameEndEdit() {
            if (DataManager.Instance != null) {
                DataManager.Instance.Save();
            }
        }
        */
        public void OnPlayPressed() {
            if (_difficultyManager == null) {
                Debug.LogError("GameSetupMenu OnPlayPressed: missing difficulty selector");
                return;
            }
            Debug.Log("Set Diffi Index to : " + _difficultyManager.CurrentIndex);
            DataManager.Instance.AiDifficultyIndex = _difficultyManager.CurrentIndex; // save difficulty ID for later use
            GameDifficultyDefinition selectedDifficulty = _difficultyManager?.GetCurrentDifficulty();
            QuestManager.Instance.SetIndex(4); // set index to intro mission index if we create a new game
            StartCoroutine(StartGameRoutine(2)); // load up the appropriate level after transition
            //StartCoroutine(StartGameRoutine(selectedDifficulty?.SceneName)); // load up the appropriate level after transition
        }

        IEnumerator StartGameRoutine(int sceneIndex) {
            //IEnumerator StartGameRoutine(string sceneName) {
            TransitionFader.PlayTransition(_startTransitionPrefab);
            yield return new WaitForSeconds(_playDelay);
            LevelLoader.LoadLevel(sceneIndex);
            InGameMenu.Open();
        }
    }
}
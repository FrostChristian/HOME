using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using HOME.Data;
using UnityEngine.SceneManagement;

namespace HOME.UI {

    public class LoadMenu : Menu<LoadMenu> {

        [Header("Text Fill")]
        [SerializeField] private string _menuHeaderLoadText = "Load";
        [SerializeField] private string _menuHeaderPlayText = "Play";
        [Space]
        [SerializeField] private string _usedSlot = "Load Game ";
        [SerializeField] private string _emptySlot = "Empty";
        [SerializeField] private string _newSlot = "New Game";
        [Space]
        [SerializeField] private string _mainMenuSceneName = "MainMenu";
        [Header("Objects")]
        [SerializeField] private GameObject _loadConfirm = default;
        [SerializeField] private GameObject[] _trashButtons = default;
        [Space]
        [SerializeField] private Text _loadConfirmText = default;
        [SerializeField] private Text _menuHeaderText = default;
        [SerializeField] private Text _currentGameText = default;
        [SerializeField] private Text[] _loadSlotText = default;
        [Space]
        [SerializeField] private Button _loadBtn = default;
        [Header("Transition")]
        [SerializeField] private TransitionFader _startTransitionPrefab = default;
        [SerializeField] private float _playDelay = 0.5f;

        public static bool _init = false;
        private string _clickedButtonText;

        protected override void Awake() {
            base.Awake();
        }
        private void OnEnable() {
            if (SceneManager.GetActiveScene().name != _mainMenuSceneName) { // if we enter through inGame refreh ui
                LoadMenuUpdateUI();
                _menuHeaderText.text = _menuHeaderLoadText;
                _loadBtn.GetComponentInChildren<Text>().text = _menuHeaderLoadText;
            } else {
                _menuHeaderText.text = _menuHeaderPlayText;
                _loadBtn.GetComponentInChildren<Text>().text = _menuHeaderPlayText;
            }
        }

        protected override void Start() {
            base.Start(); // UI SOUNDS!
            LoadMenuUpdateUI();
            for (int i = 0; i < _trashButtons.Length; i++) {
                _trashButtons[i].SetActive(false);
            }
            _init = true;
        }

        private void Update() {
            if (_init) {

                _loadConfirm.SetActive(false);
                _currentGameText.enabled = false;
                _loadBtn.interactable = false;

                for (int i = 0; i < _trashButtons.Length; i++) {
                    _trashButtons[i].SetActive(false);
                }
                _clickedButtonText = null;
                JSONSaver.SelectetSaveNumber = 0;
                LoadMenuUpdateUI();
                //Debug.Log("init: " + _init);
                _init = false;
            }
        }

        public void LoadMenuUpdateUI() {
            // save slot buttons
            for (int i = 0; i < DataManager.Instance?._saveSlots; i++) { // for each text obj
                if (File.Exists(JSONSaver.GetSaveFilePathFromDisk(i + 1))) { // check if we have save files
                    _loadSlotText[i].text = _usedSlot + (i + 1).ToString(); // fill with text
                    _loadSlotText[i].GetComponentInParent<Button>().interactable = true;
                } else {
                    if (SceneManager.GetActiveScene().name == _mainMenuSceneName) { // if we enter through the main menu scene 
                        _loadSlotText[i].text = _newSlot; // fill text with new game option
                        _loadSlotText[i].GetComponentInParent<Button>().interactable = true; // If we come in trough game then disable button

                    } else {
                        _loadSlotText[i].GetComponentInParent<Button>().interactable = false; // If we come in trough game then disable button
                        _loadSlotText[i].text = _emptySlot; // and call it empty
                    }
                }
            }

            // active game info text
            if (JSONSaver.CurrentlyActiveSavenumber != 0) {
                _currentGameText.enabled = true;
                _currentGameText.text = "Active Game:    Game " + JSONSaver.SelectetSaveNumber.ToString();
            } else {
                _currentGameText.enabled = false;
            }
        }

        public void SetSelectedSaveNumber(int saveNumber) { // gets savenumber that we want to load!
            if (saveNumber != 0) { // just for easy disableing the save button
                _loadBtn.interactable = true; // activate the save button
                JSONSaver.SelectetSaveNumber = saveNumber;
                SetTrashButtons();
            } else {
                _loadBtn.interactable = false; // deactivate save button
            }
        }

        private void SetTrashButtons() {
            for (int i = 0; i < _trashButtons.Length; i++) {
                if (JSONSaver.SelectetSaveNumber == i + 1) {
                    if (_newSlot != _clickedButtonText) {
                        _trashButtons[i].SetActive(true);
                    }
                } else {
                    _trashButtons[i].SetActive(false);
                }
            }
        }

        /// Check if clicked buttons are NewGame Or LoadGame
        public void GetButtonInfo(Text buttonText) {
            _clickedButtonText = buttonText.text;
        }

        public void OnLoadORSavePressed() {
            for (int i = 0; i < _trashButtons.Length; i++) { // disable trash buttons
                _trashButtons[i].SetActive(false);
            }

            if (_clickedButtonText == _newSlot) { // find out what we want to do
                OnNewGamePressed();
            } else {
                OnLoadPressed();
            }
        }


        public void OnNewGamePressed() {
            DataManager.Instance.Save();
            LoadMenuUpdateUI();
            GameSetupMenu.Open();
        }

        public void OnLoadPressed() {
            _loadBtn.interactable = false;
            StartCoroutine(PrepareLoad());
            DataManager.Instance.Load();
        }

        public void OnTrashClicked() { // delete savegames Button Event
            DataManager.Instance.DeleteData();
            JSONSaver.SelectetSaveNumber = 0;
            _loadBtn.interactable = false;
            SetTrashButtons();
            LoadMenuUpdateUI();
        }

        public override void OnBackPressed() {
            _init = true;
            StartCoroutine(LoadInitBeforeBack());

        }

        IEnumerator LoadInitBeforeBack() {
            yield return 2; // so it can refresh again before beeing disabeld
            base.OnBackPressed();
        }

        IEnumerator PrepareLoad() {
            _loadConfirmText.text = "Loading";
            _loadConfirm.SetActive(true);
            yield return new WaitForSecondsRealtime(0.3f);
            _loadConfirmText.text += ".";
            yield return new WaitForSecondsRealtime(0.3f);
            _loadConfirmText.text += ".";
            yield return new WaitForSecondsRealtime(0.3f);
            _loadConfirmText.text += ".";
            StartCoroutine(StartGameRoutine()); // load the level
            yield return new WaitForSecondsRealtime(0.1f);
            LoadMenuUpdateUI();
            _loadConfirm.SetActive(false);
        }

        IEnumerator StartGameRoutine() {
            TransitionFader.PlayTransition(_startTransitionPrefab);
            yield return new WaitForSecondsRealtime(_playDelay);
            LevelLoader.LoadLevel(2); // only Index numbers !!!!!
            InGameMenu.Open();
        }
    }
}

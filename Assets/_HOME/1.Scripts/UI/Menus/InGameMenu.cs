using UnityEngine;
using UnityEngine.UI;
using HOME.Data;
using System.Collections.Generic;
using System;
using HOME.Game;
using System.Collections;

namespace HOME.UI {
    public class InGameMenu : Menu<InGameMenu> {
        //------------------------------------------ Pause--------------------------------------------//
        private bool pausePressed = false;
        //------------------------------------------ +Pause--------------------------------------------//
        //------------------------------------------ Selection Panel--------------------------------------------//
        #region Selection
        [Header("Selection Info General")]
        [SerializeField] private Image _selIcon;
        [SerializeField] private Text _selName;
        [SerializeField] [Multiline] private Text _selDescription;
        [SerializeField] private Text _selOwner;
        [Header("Selection Health")]
        [SerializeField] GameObject _selHealthPanel;
        [SerializeField] private Text _selHealth;
        [SerializeField] private Slider _selHealthBar;
        [Header("Selection Inventory")]
        [SerializeField] GameObject _selInventoryPanel;
        [SerializeField] private Text _selInventoryAmountText;
        [SerializeField] private Text _selInventoryDescriptionText;
        [SerializeField] private Slider _selInventoryBar;
        [Header("Selection Buttons")]
        [SerializeField] private Button _selDestroy;
        #endregion
        //------------------------------------------+Selection Panel--------------------------------------------//
        //------------------------------------------ ResourcePanel--------------------------------------------//
        [Header("Resources Panel")]
        [SerializeField] private Text _foodField;
        [SerializeField] private Text _ironField;
        [SerializeField] private Text _alloyField;
        [SerializeField] private Text _energyField;
        [SerializeField] private Text _healthField;
        [SerializeField] private Text _distanceToHome;
        //------------------------------------------ +ResourcePanel--------------------------------------------//
        //------------------------------------------ Summary Panel--------------------------------------------//
        [Header("Summary Panel")]
        [SerializeField] private GameObject _summaryPanel;
        [SerializeField] private Slider _sliderRepProg;
        [SerializeField] private Button _buttonLeavePlanet;
        [SerializeField] private Text _stopStartRepairText;
        [SerializeField] private float _flyAwayCondition = 1f; // value between 0-1, eg 0.5f == 0.5 repair slider value
        [SerializeField] private Text _textRepProg;
        private bool _isSumActive = false;
        //planet info
        [SerializeField] private Text _planetNumber;
        [SerializeField] private Text _distToHome;
        //------------------------------------------ +Summary Panel--------------------------------------------//
        //------------------------------------------ Quests--------------------------------------------//structs
        [Header("Quest Panel")]
        [SerializeField] private GameObject _questPanel;
        [SerializeField] private QuestManager _questManager;
        [SerializeField] private Image _questIcon;
        [SerializeField] private Text _questNameText;
        [SerializeField] private Text _questNameBigText;
        [SerializeField] private Text _questDescriptionBigText;
        [SerializeField] private Text _questDescriptionText;
        [SerializeField] private Text _questProgressText;
        [SerializeField] private bool _isQuestActive = false; // just for quest panel toggle
        public static bool _initQuests = false;
        //------------------------------------------ +Quests--------------------------------------------//

        //------------------------------------------ Actions/Buttons--------------------------------------------//
        [Header("Actions / Buttons")]
        [SerializeField] private Button[] _buttons;
        private List<Action> actionCalls = new List<Action>();

        //------------------------------------------ +Actions/Buttons--------------------------------------------//
        #region MonoBehavior
        private void OnEnable() {
            Time.timeScale = 1; // if we come from a menu reset to 1
        }

        protected override void Start() {
            base.Start();
            DataManager.Instance.OnResourceAmountChanged += delegate (object sender, EventArgs e) {
                ResourceInfo();
            };
            ResourceInfo();

            SelectionClear(); // clear selection at the start
            for (int i = 0; i < _buttons.Length; i++) {//clear Buttons
                var index = i;
                _buttons[index].onClick.AddListener(delegate () { // index buttons
                    OnButtonClick(index);
                });
            }
            ClearButtons();
            _questManager = GetComponent<QuestManager>();
            SelectQuest(4);
        }

        private void Update() {
            if (DataManager.Instance != null) {
                PlayerInfo();
                RepairInfo(); // needs repeating
                SummaryInfo();// only do this once
            }

            if (_initQuests) {
                StartCoroutine(CheckQuestCoroutine());
                _initQuests = false;
            }
            if (Input.GetKeyDown(KeyCode.Escape)) {
                OnPausePressed();
            }

        }
        #endregion
        //------------------------------------------ <Update UI Info>--------------------------------------------//
        #region Update Info
        private void ResourceInfo() {
            _foodField.text = "Food: " + Math.Round(DataManager.Instance.GetResourceAmount(DataManager.ResourceType.Food), 0); //Resources
            _ironField.text = "Iron: " + DataManager.Instance.GetResourceAmount(DataManager.ResourceType.IronOre); //Resources
            _alloyField.text = "Alloy: " + DataManager.Instance.GetResourceAmount(DataManager.ResourceType.BauxiteOre); //Resources
            _energyField.text = "Energy: " + DataManager.Instance.GetResourceAmount(DataManager.ResourceType.Energy); //Resources
        }

        private void PlayerInfo() {
            _healthField.text = "Health: " + DataManager.Instance.PlayerShipHealth + " <3"; //Resources
            _distanceToHome.text = "Home is: " + DataManager.Instance.HomeDistance + " LJ away!"; //Resources
        }

        private void RepairInfo() {
            QuestInfo();
            _textRepProg.text = DataManager.Instance.PlayerShipHealth + " / " + DataManager.Instance.PlayerShipMaxHealth; // set slider text
            _sliderRepProg.value = DataManager.Instance.PlayerShipHealth / DataManager.Instance.PlayerShipMaxHealth; // set slider value
            if (DataManager.Instance.PlayerShipHealth >= DataManager.Instance.PlayerShipMaxHealth) { // if we are full health
                _sliderRepProg.value = 1f; // set slider to 1
            }
            if (_sliderRepProg.value > _flyAwayCondition) {///////////////////////////////////////////////////////////////////////////////////////////////////////////Fly Away Button//////////////
                _buttonLeavePlanet.interactable = true;
            } else {
                _buttonLeavePlanet.interactable = false;
            }
        }

        private void SummaryInfo() {
            _planetNumber.text = ": " + (DataManager.Instance.PlanetsVisited.Count + 1).ToString();
            _distToHome.text = ": " + DataManager.Instance.HomeDistance.ToString() + " LY";
        }
        #endregion
        //------------------------------------------ +<Update UI Info>--------------------------------------------//
        //------------------------------------------ Pause--------------------------------------------//
        public void OnPausePressed() {
            Time.timeScale = 0;
            PauseMenu.Open();
            Debug.Log("InGameMenu OnPausePressed(): PauseMenu.Open()");
        }
        //------------------------------------------ + Pause--------------------------------------------//
        //------------------------------------------ Infopanel--------------------------------------------//
        #region Infopanel
        public void SelectionDestroy() {
            foreach (var sel in MouseManager.Instance.selectedUnits) {
                sel.GetComponent<Entity>().DestroyEntity();
            }
        }

        public void SelectionFill(string unitName, bool unitHasHealth, string unitHealth, string unitDescription, Sprite unitIcon, string unitOwner, float unitHealthBar, bool unitHasResources, string unitInventoryDesc, float unitInventoryBar, string unitInventory) {

            _selName.text = unitName;
            _selDescription.text = unitDescription;
            _selIcon.sprite = unitIcon;
            _selIcon.color = Color.white;
            _selOwner.text = unitOwner;
            _selDestroy.gameObject.SetActive(true);

            if (unitHealth != "" && unitHasHealth) { // if unit has health
                _selHealthPanel.gameObject.SetActive(true);
                _selHealthBar.value = unitHealthBar;
                _selHealth.text = unitHealth;
            }

            if (unitInventory != "" && unitHasResources) {// if unit has resources
                _selInventoryPanel.gameObject.SetActive(true);
                _selInventoryDescriptionText.text = unitInventoryDesc;
                _selInventoryBar.value = unitInventoryBar;
                _selInventoryAmountText.text = unitInventory;
            }
        }

        public void SelectionClear() {
            SelectionFill("", false, "", "", null, "", 0f, false, "", 0f, "");
            _selIcon.color = Color.clear;
            _selHealthPanel.gameObject.SetActive(false);
            _selInventoryPanel.gameObject.SetActive(false);
            _selDestroy.gameObject.SetActive(false);
        }
        #endregion
        //------------------------------------------+Infopanel--------------------------------------------//
        //------------------------------------------ Repair/Summary Panel--------------------------------------------//
        public void OnFlyAwayPressed() {
            GameManager.Instance.iWon = true;
            GameManager.Instance.IsGameOver = true;
            _summaryPanel.SetActive(false);
            Debug.Log("InGameMenu: OnFlyAwayPressed() On Fly Away Pressed!");
        }

        public void OnStartStopRepairPressed() {
            if (GameManager._isRepairActive) {
                GameManager._isRepairActive = false;
                _stopStartRepairText.text = "Start \n Repair!";
            } else {
                GameManager._isRepairActive = true;
                _stopStartRepairText.text = "Stop \n Repair!";
            }
        }

        public void OnSummaryPressed() {
            if (_isSumActive == false) {
                _summaryPanel.SetActive(true);
                _isSumActive = true;
            } else {
                _summaryPanel.SetActive(false);
                _isSumActive = false;
            }
        }
        //------------------------------------------ +Repair/Summary Panel--------------------------------------------//
        //--------------------------------------------------------------------------------------------//
        //------------------------------------------ Quests--------------------------------------------//
        public void OnQuestPressed() { //on button
            if (_isQuestActive == true) { // if active on click close it
                _questPanel.SetActive(false);
                _isQuestActive = false;
                Debug.Log("false" + _isQuestActive);
            } else {
                _questPanel.SetActive(true);
                _isQuestActive = true;
                Debug.Log("true" + _isQuestActive);
            }
        }

        public void SelectQuest(int levelIndex) {
            _questManager.SetIndex(levelIndex);
            QuestInfo(); // load questinfo UI
        }

        public void OnQuestCompleate(int levelIndex) {
            _questPanel.SetActive(true);
            _questManager.SetIndex(levelIndex);
            QuestInfo(); // load questinfo UI
        }
        public void OnOkayQuestPressed() {
            QuestSetupDefinition currentQuest = _questManager?.GetCurrentQuest();
            if (currentQuest.Id == 7) {
                _isQuestActive = false;
                return;
                
            }
            if (currentQuest.Id == 3) {
                SelectQuest(7);
                QuestInfo();
            }
            if (currentQuest.Id == 4 || currentQuest.Id == 5 ) {
                _questManager.IncrementIndex();
                QuestInfo();
            } else if (currentQuest.Id == 6) {
                SelectQuest(0);
                QuestInfo();
            } else {
                _isQuestActive = true;
                OnQuestPressed(); // exit
            }
        }

        public void OnPreviousQuestPressed() {
            QuestManager.Instance.DecrementIndex();
            QuestInfo();
        }


        public IEnumerator CheckQuestCoroutine() {
            while (GameManager.Instance != null) {
                CheckQuestProgress();
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void CheckQuestProgress() {

            QuestSetupDefinition currentQuest = _questManager?.GetCurrentQuest();
            if (currentQuest.Id == 0) {// build one microgreenery
                foreach (var player in GameManager.Instance.Factions) { // look for the players units
                    if (!player.isAi) {
                        foreach (var entity in player.ActiveUnits) {
                            if (entity.gameObject.tag == "FoodEntity") {
                                OnQuestCompleate(1);
                                return;
                            }
                        }
                    }
                }
            }
            if (currentQuest.Id == 1) {// build one solar farm
                foreach (var player in GameManager.Instance.Factions) { // look for the players units
                    if (!player.isAi) {
                        foreach (var entity in player.ActiveUnits) {
                            if (entity.gameObject.tag == "EnergyEntity") {
                                OnQuestCompleate(2);
                                return;
                            }
                        }
                    }
                }
            }
            if (currentQuest.Id == 2) {// build 3 turrets
                foreach (var player in GameManager.Instance.Factions) { // look for the players units
                    if (!player.isAi) {
                        foreach (var entity in player.ActiveUnits) {
                            Debug.Log("" + entity.name);
                            if (entity.gameObject.tag == "DefenseEntity") {
                                currentQuest.Progress++;
                            }
                            if (currentQuest.Progress > 2) {
                                OnQuestCompleate(3);
                                currentQuest.Progress = 0;
                            }
                            QuestInfo();
                            return;
                        }
                    }
                }
            }
            if (currentQuest.Id == 3) {// repair ship
                currentQuest.Progress = (int)DataManager.Instance.PlayerShipHealth;
                if (DataManager.Instance.PlayerShipHealth == DataManager.Instance.PlayerShipMaxHealth) {
                    OnQuestCompleate(4);
                    return;
                }
            }
            if (currentQuest.Id == 4) {// intro
                return;
            }
            if (currentQuest.Id == 5) {// build one microgreenery
                return;
            }
            QuestInfo();
        }

        public void QuestInfo() {

            QuestSetupDefinition currentQuest = _questManager?.GetCurrentQuest();
            if (currentQuest == null) {
                return;
            }
            // QuestQuickViewPanel
            _questIcon.sprite = currentQuest.Icon;
            _questNameText.text = currentQuest.Name;
            _questDescriptionText.text = currentQuest.DescriptionShort;
            _questProgressText.text = currentQuest.Progress.ToString() + " / " + currentQuest.Condition.ToString();

            // Quest Show Panel
            _questNameBigText.text = currentQuest.Name;
            _questDescriptionBigText.text = currentQuest.DescriptionLong;
            //_lockIcon.gameObject.SetActive(false);   // check lock status, currently defaulting to off
        }
        //------------------------------------------ +Quests--------------------------------------------//
        //------------------------------------------ Actions/Buttons--------------------------------------------//
        #region Action/Buttons
        public void ClearButtons() {
            foreach (Button b in _buttons) {
                b.gameObject.SetActive(false);
                if (b.gameObject.TryGetComponent(out UIEvents events)) { // if we have a UIEvent on this button destroy it
                    events.Destroy();
                }
            }

            actionCalls.Clear();
        }

        public void AddButton(Sprite icon, Action onClick, Entity entity) {
            int index = actionCalls.Count;
            _buttons[index].gameObject.SetActive(true);
            _buttons[index].GetComponent<Image>().sprite = icon; // set button entity item
            _buttons[index].gameObject.AddComponent<UIEvents>(); // add UIEvents for PointerEnter PointerExit Tooltip

            TooltipEntitys.AddTooltip(
                _buttons[index].gameObject.transform,
                entity.GetIcon,
                entity.GetName,
                entity.GetDescription,
                entity.GetIronCost,
                entity.GetFoodCost,
                entity.GetAlloyCost,
                entity.GetEnergyCost);

            actionCalls.Add(onClick); // add onClick
        }

        public void OnButtonClick(int index) {
            actionCalls[index]();
        }
        #endregion
        //------------------------------------------ +Actions/Buttons--------------------------------------------//
        //------------------------------------------ Debug--------------------------------------------//
        public void DebugWinBtn() {
            var de = FindObjectOfType<GameManager>();
            de.DebugWin = true;
            Debug.Log("Debug WIn");
        }

        public void DebugMiscBtn() {
            var de = FindObjectOfType<GameManager>();
            de.FindSpawnPoints();
            Debug.Log("Debug FindSpawnPlayers");
        }
        //------------------------------------------ +Debug--------------------------------------------//
        //--------------------------------------------------------------------------------------------//
    }
}

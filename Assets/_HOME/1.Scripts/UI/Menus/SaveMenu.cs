using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using HOME.Data;
using System;

namespace HOME.UI {

    public class SaveMenu : Menu<SaveMenu> { // basically copy of LoadMenu, more comments there!

        private string _emptySlot = "Save New";
        private string _usedSlot = "Replace Game ";

        [SerializeField] private GameObject _saveConfirm = default;
        [SerializeField] private Text _saveConfirmText = default;
        [SerializeField] private Text _currentGameText = default;
        [SerializeField] private GameObject[] _trashButtons = default;

        [SerializeField] private Text[] _saveSlotText = default;
        [SerializeField] private Button _saveBtn = default;

        public static bool _initS = false;

        protected override void Start() {
            base.Start(); // UI SOUNDS!

            _saveConfirm.SetActive(false);
            _saveBtn.interactable = false;

            for (int i = 0; i < _trashButtons.Length; i++) {
                _trashButtons[i].SetActive(false);
            }
            _initS = true;
            UpdateUI();
        }

        private void Update() {
            if (_initS) {

                _saveConfirmText.enabled = false;
                _currentGameText.enabled = false;
                _saveBtn.interactable = false;

                for (int i = 0; i < _trashButtons.Length; i++) {
                    _trashButtons[i].SetActive(false);
                }

                JSONSaver.SelectetSaveNumber = 0;
                UpdateUI();
                Debug.Log("initS: " + _initS);
                _initS = false;
            }
        }

        void UpdateUI() {
            // save slot buttons
            for (int i = 0; i < DataManager.Instance._saveSlots; i++) {
                if (File.Exists(JSONSaver.GetSaveFilePathFromDisk(i + 1))) {
                    _saveSlotText[i].text = _usedSlot + (i + 1).ToString();
                } else {
                    _saveSlotText[i].text = _emptySlot;
                }
            }
            // active game info
            if (JSONSaver.CurrentlyActiveSavenumber != 0) {
                _currentGameText.enabled = true;
                _currentGameText.text = "Active Game:    Game " + JSONSaver.SelectetSaveNumber.ToString();
            } else {
                _currentGameText.enabled = false;
            }
        }

        public void SetSelectedSaveNumber(int saveNumber) { // gets savenumber that we want to load!
            if (saveNumber != 0) { // just for easy disableing the save button
                _saveBtn.interactable = true; // activate the save button
                JSONSaver.SelectetSaveNumber = saveNumber;
                SetTrashButtons();
            } else {
                _saveBtn.interactable = false; // deactivate save button
            }
        }

        private void SetTrashButtons() { // find trash button for selected button
            for (int i = 0; i < _trashButtons.Length; i++) {
                if (JSONSaver.SelectetSaveNumber == i + 1) {
                    _trashButtons[i].SetActive(true);
                } else {
                    _trashButtons[i].SetActive(false);
                }
            }
        }

        public void OnTrashClicked() { // delete savegames Button Event
            DataManager.Instance.DeleteData();
            JSONSaver.SelectetSaveNumber = 0;
            _saveBtn.interactable = false;
            SetTrashButtons();
            UpdateUI();
        }

        public override void OnBackPressed() {
            UpdateUI();
            base.OnBackPressed();
        }

        public void OnSavePressed() {
            _saveBtn.interactable = false;
            StartCoroutine(ConfirmSaveing());
            DataManager.Instance.Save();
        }

        IEnumerator ConfirmSaveing() {
            _saveConfirmText.text = "SAVING";
            _saveConfirmText.enabled = true;
            _saveConfirm.SetActive(true);
            yield return new WaitForSecondsRealtime(0.3f);
            _saveConfirmText.text += ".";
            yield return new WaitForSecondsRealtime(0.3f);
            _saveConfirmText.text += ".";
            yield return new WaitForSecondsRealtime(0.3f);
            _saveConfirmText.text += ".";
            yield return new WaitForSecondsRealtime(0.1f);
            _saveConfirm.SetActive(false);
            _saveConfirmText.enabled = false;

            UpdateUI();
        }
    }
}

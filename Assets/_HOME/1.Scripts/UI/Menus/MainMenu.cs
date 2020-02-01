using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HOME;
using HOME.Data;

namespace HOME.UI {

    public class MainMenu : Menu<MainMenu> {

        private DataManager _dataManager;
        [SerializeField] private InputField _inputField = default;

        protected override void Awake() {
            base.Awake();
            _dataManager = FindObjectOfType<DataManager>();
        }

        protected override void Start() {
            base.Start();            
        }
      
        public void OnPlayPressed() {
            OnLoadPressed();
        }

        public void OnLoadPressed() {
            LoadMenu._init = true;
            LoadMenu.Open();
        }

        public void OnOptionsPressed() {
            OptionsMenu.Open();
        }

        public void OnCreditsPressed() {
            CreditsMenu.Open();
        }

        public override void OnBackPressed() {
            Application.Quit();
        }
    }
}
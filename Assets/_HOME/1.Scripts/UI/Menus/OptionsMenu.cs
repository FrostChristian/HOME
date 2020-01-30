using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HOME.Data;

namespace HOME.UI {

    public class OptionsMenu : Menu<OptionsMenu> {

        [Header("Sliders")]
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;

        protected override void Start() {
            base.Start();
            LoadPlayerPrefs(); // load player prefs
        }

        public void OnMasterVolumeChanged(float volume) { //slider ingame
          /*  if (DataManager.Instance != null) {
                DataManager.Instance.MasterVolume = volume;
                
            }*/
            PlayerPrefs.SetFloat("MasterVolume", volume); //save
        }
        public void OnSFXVolumeChanged(float volume) {
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }
        public void OnMusicVolumeChanged(float volume) {
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }

        public override void OnBackPressed() {
            base.OnBackPressed();
            PlayerPrefs.Save(); //save to disk on back click

        }

        public void LoadPlayerPrefs() {
            _masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");//set sliders
            _sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume");
            _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
    }
}
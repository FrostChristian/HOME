using UnityEngine;
using UnityEngine.UI;
using HOME.Game;

namespace HOME.UI {

    [RequireComponent(typeof(Canvas))]

    public abstract class Menu<T> : Menu where T : Menu<T> { // CRTP curiosly recurring template pattern 

        private static T _instance;
        public static T Instance { get { return _instance; } }

        protected virtual void Awake() {
            if (_instance != null) { // singelton
                Destroy(gameObject);
            } else {
                _instance = (T)this;
            }
        }

        protected virtual void OnDestroy() {
            _instance = null;
        }

        public static void Open() {
            if (MenuManager.Instance != null && Instance != null) {
                MenuManager.Instance.OpenMenu(Instance);
            }
        }
    }

    public abstract class Menu : MonoBehaviour { // base for menus

        protected virtual void Start() {
            // adding sounds to ALL Menu buttons and sliders in here /// call base.Start!!!
            Button[] _buttons = GetComponentsInChildren<Button>(true);
            for (int i = 0; i < _buttons.Length; i++) {
                UIEvents audio = _buttons[i].gameObject.AddComponent<UIEvents>();// Add ButtonUI helper to the button
                audio.theButton = _buttons[i];
            }

            Slider[] _sliders = GetComponentsInChildren<Slider>(true);
            for (int i = 0; i < _sliders.Length; i++) {
                UIEvents audio = _sliders[i].gameObject.AddComponent<UIEvents>();// Add ButtonUI helper to the button
                audio.theSlider = _sliders[i]; // Add sounds
            }
        }

        public virtual void OnBackPressed() {
            MenuManager menuManager = FindObjectOfType<MenuManager>();
            if (menuManager != null) {
                menuManager.CloseMenu();
            }
        }
    }
}

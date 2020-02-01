using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace HOME.UI {

    public class MenuManager : MonoBehaviour {
        [Header("Menu Prefabs")]
        [SerializeField] private MainMenu mainMenuPrefab = default;
        [SerializeField] private LoadMenu loadMenuPrefab = default;
        [SerializeField] private SaveMenu saveMenuPrefab = default;
        [SerializeField] private OptionsMenu optionsMenuPrefab = default;
        [SerializeField] private CreditsMenu creditsScreenPrefab = default;
        [SerializeField] private GameSetupMenu gameSetupMenu = default;
        [SerializeField] private InGameMenu inGameMenuPrefab = default;
        [SerializeField] private PauseMenu pauseMenuPrefab = default;
        [SerializeField] private PlanetSelectMenu planetSelectMenu = default;
        [SerializeField] private WinMenu winMenuPrefab = default;
        [SerializeField] private LoseMenu loseMenuPrefab = default;

        [Space]
        [SerializeField] private Transform _menuParent = default;

        [Header("Menu Button Sounds")]
        public AudioSource audioSource;
        public AudioClip mouseHover;
        public AudioClip mouseClick;
        public AudioClip mouseDrag;
        private float _soundDelay = 0.05f;

        private Stack<Menu> _menuStack = new Stack<Menu>();

        private static MenuManager _instance;
        public static MenuManager Instance { get => _instance; }

        private void Awake() {
            if (_instance != null) {//singleton
                Destroy(gameObject);
            } else {
                _instance = this; // global
                InitializeMenus();
                DontDestroyOnLoad(gameObject); //keep active between scenes obv
            }

            audioSource = gameObject.GetComponentInParent<AudioSource>(); //.GetComponent<AudioSource>();
            if (audioSource == null) {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        private void OnDestroy() {
            if (_instance == this) {
                _instance = null;
            }
        }

        private void InitializeMenus() {
            if (_menuParent == null) {
                GameObject menuParentObject = new GameObject("Menus");
                _menuParent = menuParentObject.transform;
            }
            DontDestroyOnLoad(_menuParent.gameObject);
            // Menu[] menuPrefabs = { mainMenuPrefab, loadMenuPrefab, optionsMenuPrefab, creditsScreenPrefab, inGameMenuPrefab,  pauseMenuPrefab, winMenuPrefab };

            BindingFlags myFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            FieldInfo[] fields = this.GetType().GetFields(myFlags);

            foreach (FieldInfo field in fields) {
                Menu prefab = field.GetValue(this) as Menu;
                if (prefab != null) {
                    Menu menuInstance = Instantiate(prefab, _menuParent);
                    if (prefab != mainMenuPrefab) {
                        menuInstance.gameObject.SetActive(false);
                    } else {
                        OpenMenu(menuInstance);
                    }
                }
            }
        }

        public void OpenMenu(Menu menuInstance) {
            if (menuInstance == null) {
                Debug.Log("MENUMANAGER OpenMenu ERROR: invalid menu");
                return;
            }

            if (_menuStack.Count > 0) {
                foreach (Menu menu in _menuStack) {
                    menu.gameObject.SetActive(false);
                }
            }
            menuInstance.gameObject.SetActive(true);
            _menuStack.Push(menuInstance);
        }

        public void CloseMenu() {
            if (_menuStack.Count == 0) {
                Debug.LogWarning("MENUMANAGER CloseMenu ERROR: No menus in stack!");
                return;
            }

            Menu topMenu = _menuStack.Pop();
            topMenu.gameObject.SetActive(false);

            if (_menuStack.Count > 0) {
                Menu nextMenu = _menuStack.Peek();
                nextMenu.gameObject.SetActive(true);
            }
        }

        public void PlayClickSound() {
            StartCoroutine(PlayPointerClick());
        }

        private IEnumerator PlayPointerClick() {
            audioSource.priority = 10;
            audioSource.PlayOneShot(mouseClick);
            yield return new WaitForSeconds(_soundDelay);
        }
    }
}
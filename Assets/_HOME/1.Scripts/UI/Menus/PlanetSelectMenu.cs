using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using HOME.Data;
using HOME.Game;

namespace HOME.UI {

    public class PlanetSelectMenu : Menu<PlanetSelectMenu> {

        [Header("UI Elements")]
        public GameObject Panel;
        public GameObject ParentPanel;
        public GameObject confirm;
        public Button playButton;
        //------------------------------------------ Planet Info--------------------------------------------//
        [Header("Planet Info")]
        [SerializeField] private Text _planetInfo = default;
        [SerializeField] private Text _planetNameForConfirmPanel = default;
        [SerializeField] private Text _planetDamageToShip = default;
        [SerializeField] private Text _planetDistanceToShip = default;
        [SerializeField] private List<GameObject> _planetPanels = new List<GameObject>();
        //------------------------------------------ +Planet Info--------------------------------------------//
        //------------------------------------------ ShipInfo--------------------------------------------//
        [Header("Player Info")]
        [SerializeField] private Text _textPlayerDamage = default;
        [SerializeField] private Text _textShipDistanceToHome = default;
        [SerializeField] private Text _textPlayerName = default;
        [SerializeField] private float _calcPlayerDamage = default;
        [SerializeField] private float _calcShipDistanceToHome = default;
        //------------------------------------------ +ShipInfo--------------------------------------------//
        //------------------------------------------ Transitions--------------------------------------------//
        [SerializeField] private float _playDelay = 1f;
        [SerializeField] private PlanetTransitionFader startPlanetTransitionPrefab = default;
        [SerializeField] private TransitionFader startTransitionPrefab = default;
        [SerializeField] private static CanvasGroup _canvasGroup_Static = default;
        public static CanvasGroup CanvasGroup_Static { get => _canvasGroup_Static; set => _canvasGroup_Static = value; }
        [SerializeField] private Transform _planetSelected = default;
        public Transform PlanetSelected { get => _planetSelected; set => _planetSelected = value; }
        //------------------------------------------ +Transitions--------------------------------------------//

        protected override void Awake() {
            base.Awake();
            _canvasGroup_Static = GetComponent<CanvasGroup>();
            playButton.interactable = false;
        }

        public void InstantiateUIPlanetPanel(string info, float dmgToShip, float distToPlanet, int planetID, Transform planet) {
            _planetInfo.text = info;
            _planetDamageToShip.text = "Damage to Ship: \n ~" + dmgToShip.ToString() + " %";
            _planetDistanceToShip.text = "Distance: \n" + distToPlanet.ToString() + " Light Years";
            var go = Instantiate(Panel, ParentPanel.transform); // Instantiate Panel
            go.SetActive(true); // SETACTIVE FALSE OR DESTROY
            go.name = "PanelÍD" + planetID;
            var btn = go.GetComponentInChildren<Button>(); //get btn ref
            btn.onClick.AddListener(() => {//add onClick event return PlanetID of corresponding button
                PlanetManager.Instance.SelectPlanet(planetID);
                InfoUpdate(dmgToShip, distToPlanet, planet, info);                // updates current player info in UI
                playButton.interactable = true; // enable the play game button
            });
            _planetPanels.Add(go);
        }

        private void ClearUIPlanetPanel() {
            foreach (var p in _planetPanels) {
                Destroy(p.gameObject);//destroy panels in list
            }
            _planetPanels.Clear(); //clear panel list
        }

        private void InfoUpdate(float dmgToShip, float distToPlanet, Transform planet, string info) {
            _planetSelected = planet;
            _textPlayerDamage.text = "Ship Damage: " + DataManager.Instance.PlayerShipHealth + " - " + dmgToShip + "%";
            _calcPlayerDamage = DataManager.Instance.PlayerShipHealth - dmgToShip;
            _textShipDistanceToHome.text = "Distance to Home: " + DataManager.Instance.HomeDistance.ToString() + " - " + distToPlanet + " LJ";
            _calcShipDistanceToHome = DataManager.Instance.HomeDistance - distToPlanet;
            _textPlayerName.text = "Player:  " + DataManager.Instance.PlayerName.ToString();
            _planetNameForConfirmPanel.text = "Travel to:\n" + info + " ?";
        }

        //------------------------------------------ Button Events--------------------------------------------//

        public void OnPlayPressed() {
            confirm.SetActive(true);
        }

        public void OnBackgroundClickedPlanetReset() {
            playButton.interactable = false;
            PlanetManager.Instance.SelectPlanet(0); // null to deselect selected planet
        }

        public void OnConfirmPlayPressed() {
            playButton.interactable = false;
            DataManager.Instance.HomeDistance = _calcShipDistanceToHome;
            DataManager.Instance.PlayerShipHealth = _calcPlayerDamage;
            StartCoroutine(OnPlayPressedRoutine());
            confirm.SetActive(false);
        }

        private IEnumerator OnPlayPressedRoutine() {
            PlanetTransitionFader.PlayTransition(startPlanetTransitionPrefab, _planetSelected);
            yield return new WaitForSeconds(_playDelay);
            TransitionFader.PlayTransition(startTransitionPrefab);
            ClearUIPlanetPanel();
            yield return new WaitForSeconds(_playDelay);

            LevelLoader.LoadLevel(2); // load with index 
            InGameMenu.Open();
            Destroy(PlanetManager.Instance.gameObject); // dont need planet manager anymore
            Debug.Log("PlanetSelectMenu: OnPlayPressedRoutine() finished!");
        }

        public override void OnBackPressed() { // save here !
            base.OnBackPressed();
            if (DataManager.Instance != null) {
                DataManager.Instance.Save(); //save to disk
            }
        }
        //------------------------------------------ +Button Events--------------------------------------------//
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HOME.Data;
using HOME.UI;
using System;
using UnityEngine.SceneManagement;

namespace HOME.Game {

    public class PlanetManager : MonoBehaviour {

        //------------------------------------------ PlanetInfo--------------------------------------------//
        private static PlanetManager _instance;
        public static PlanetManager Instance { get { return _instance; } }

        public List<PlanetSetupDefinition> RandomPlanets = new List<PlanetSetupDefinition>(); // hilds spawned planet info
        [SerializeField] private List<Transform> _spawnLocation = new List<Transform>(); // spawn location in scene
        [SerializeField] private List<GameObject> _randomPlanet = new List<GameObject>(); // holds all planet prefabs

        // Holding Planet Info in these:
        [SerializeField] private PlanetSelectMenu _planetSelectMenu = default;
        private int _planetID;
        private string _planetInfo;
        private float _planetDistanceToShip;
        private float _planetDamageToShip;
        private Transform _planetLocation;
        private bool _planetSelected;
        //------------------------------------------ +PlanetInfo--------------------------------------------//
        private void Awake() { // not really nessesary
            if (_instance == null) {
                _instance = this;
            } else {
                Destroy(_instance);
            }
            if (SceneManager.GetActiveScene().name == "PlanetSelect") {
                PlanetSelectMenu.CanvasGroup_Static.alpha = 1; // Set canvas alpha to 1 if coming from an old map
                PlanetSelectMenu.Open();
                Debug.Log("PlanetManager Awake(): Entered Game through PlanetSelect");
            }
            _planetSelectMenu = FindObjectOfType<PlanetSelectMenu>();
        }
        private void Start() {
            FindPlanetSpawns();
            CreateRandomPlanets();
        }

        private void FindPlanetSpawns() {
            var pSpawn = GameObject.FindGameObjectsWithTag("PlanetSpawn");
            foreach (var hS in pSpawn) {
                _spawnLocation.Add(hS.transform);
            }
        }
        private void CreateRandomPlanets() { //Planets for PlanetSelect screen
            if (_planetSelectMenu == null) {
                _planetSelectMenu = FindObjectOfType<PlanetSelectMenu>();
                Debug.Log("PLANETMANAGER CreateRandomPlanets() NO planetSelectMenu!");
                return;
            }
            foreach (var s in _spawnLocation) {// create Planet for each spawnpoint
                GameObject rand = _randomPlanet[UnityEngine.Random.Range(0, _randomPlanet.Count)]; // if it's a List
                var planetObj = Instantiate(rand, s);// instanciate dummy planet in scene

                //set random values
                _planetID = UnityEngine.Random.Range(1000000, 9999999); // just pick a high number thats unlikely to be picked again
                _planetInfo = GetRandomPlanetName();
                int tempDistVar = UnityEngine.Random.Range(5, 20); // store dist to calc damage correlating to distance
                _planetDistanceToShip = tempDistVar;
                float tempDamage = UnityEngine.Random.Range(tempDistVar * 2f, tempDistVar * 3); // get random damage roughly eqivalent to distance!
                _planetDamageToShip = (float)Math.Round(tempDamage, 2); // round to two decimals and cast to float
                _planetLocation = planetObj.transform;
                _planetSelected = false;

                RandomPlanets.Add(new PlanetSetupDefinition( // new Planet
                        _planetID,
                        _planetInfo,
                        _planetDistanceToShip,
                        _planetDamageToShip,
                        _planetLocation,
                        _planetSelected));

                _planetSelectMenu.InstantiateUIPlanetPanel(// instantiate UI Panels with new Planet Values
                    _planetInfo,
                    _planetDamageToShip,
                    _planetDistanceToShip,
                    _planetID,
                    _planetLocation);
            }
        }

        public void SelectPlanet(int selectedPlanetID) { //onClick() getting selected Planet from UI
            foreach (var planet in RandomPlanets) {
                if (selectedPlanetID == planet.planetID && planet.planetSelected) { // if we already selected retrun
                }
                if (selectedPlanetID == planet.planetID && !planet.planetSelected) { // if ID matches ID from List
                    planet.planetSelected = true; // set selected to true
                    DataManager.Instance.PlanetsVisited.Add(planet);
                    //Add to permanent planet list
                } else {
                    planet.planetSelected = false; //if new planet selected set others to false
                    DataManager.Instance.PlanetsVisited.Remove(planet);
                    //remove from permanent planet list
                }
            }
        }

        private string GetRandomPlanetName() {
            int index = UnityEngine.Random.RandomRange(0, DataManager.Instance.planetNames.Count);
            string indexName = DataManager.Instance.planetNames[index];
            DataManager.Instance.planetNames.Remove(DataManager.Instance.planetNames[index]);
            return indexName;
        }

        private void OnDestroy() {
            _spawnLocation.Clear();
        }
    }
}

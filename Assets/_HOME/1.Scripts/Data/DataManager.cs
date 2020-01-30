using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using HOME.Game;

namespace HOME.Data {
    public class DataManager : MonoBehaviour {

        private static DataManager _instance;
        public static DataManager Instance { get => _instance; }

        private SaveData _saveData;
        private JSONSaver _jsonSaver;

        //------------------------------------------Options--------------------------------------------//
        public float MasterVolume { get { return _saveData.masterVolume; } set {  _saveData.masterVolume = value; } }
        public float SFXVolume { get { return _saveData.sfxVolume; } set {  _saveData.sfxVolume = value; } }
        public float MusicVolume { get { return _saveData.musicVolume; } set {  _saveData.musicVolume = value; } }

        //-----------------------------------------+Options--------------------------------------------//
        //------------------------------------------+PLayer--------------------------------------------//
        public string PlayerName { get { return _saveData.pName; } set { Debug.Log("Set Player Name to: " + (_saveData.pName = value)); _saveData.pName = value; } }
        public float PlayerShipHealth { get { return _saveData.pShipHealth; } set { _saveData.pShipHealth = value; } }
        public float PlayerShipMaxHealth { get { return _saveData.pShipMaxHealth; } set { _saveData.pShipMaxHealth = value; } }
        //Resources
        public float PlayerCredits { get { return _saveData.pCredits; } set { _saveData.pCredits = value; } }
        public float PlayerIronOre { get { return _saveData.pIronOre; } set { _saveData.pIronOre = value; } }

        //------------------------------------------+PLayer--------------------------------------------//
        //------------------------------------------ AI --------------------------------------------//
        public int AiDifficultyIndex { get => _saveData.aiDifficulty; set => _saveData.aiDifficulty = value; }

        public string AIName { get { return _saveData.aiName; } set { Debug.Log("Set ai Name to: " + (_saveData.aiName = value)); _saveData.aiName = value; } }
        public float AICredits { get { return _saveData.aiCredits; } set { _saveData.aiCredits = value; } }
        public float AIHealth { get { return _saveData.aiHealth; } set { Debug.Log("Set ai Health"); _saveData.aiHealth = value; } }

        //------------------------------------------+AI--------------------------------------------//
        //------------------------------------------Quest--------------------------------------------//
        //------------------------------------------+Quest--------------------------------------------//
        //------------------------------------------ GameInfo--------------------------------------------//
        public float HomeDistance { get { return _saveData.homeDistance; } set { Debug.Log("Set Home Distance to: " + (_saveData.homeDistance = value)); _saveData.homeDistance = value; } }

        //------------------------------------------+GameInfo--------------------------------------------//
        //------------------------------------------ Planets--------------------------------------------//
        //public List<PlanetSetupDefinition> PlanetsVisited { get { return _saveData.planetsVisited; } set {  _saveData.planetsVisited = value; } }
        public List<string> planetNames = new List<string> { "Oxan", "Thena", "Isa", "Venture", "Plexor", "Maximore", "Pic", "Xlas", "Juno", "Noctine", "Tropic", "Trine", "Waterborne", "Rustic", "Plant North", "Starlet" };

        [SerializeField] private List<PlanetSetupDefinition> _planetsVisited = new List<PlanetSetupDefinition>();
        public List<PlanetSetupDefinition> PlanetsVisited { get { return _planetsVisited; } set { _planetsVisited = value; } }
        //------------------------------------------ +Planets--------------------------------------------//
        //------------------------------------------ Resource Gatherer--------------------------------------------//
        public  event EventHandler OnResourceAmountChanged;

        public Dictionary<ResourceType, float> resourceAmountDictionary;
 
        public enum ResourceType {
            Empty,
            Iron,
            Alloy,
            Food,
            Energy,
            BauxiteOre,
            IronOre,
        }
        public void Init() {
            resourceAmountDictionary = new Dictionary<ResourceType, float>();

            foreach (ResourceType resourceType in System.Enum.GetValues(typeof(ResourceType))) { 
                resourceAmountDictionary[resourceType] = 0;// init all keys with 0
            }
        }

        public void AddResourceAmount(ResourceType resourceType, float amount) {
            resourceAmountDictionary[resourceType] += amount;
            OnResourceAmountChanged?.Invoke(null, EventArgs.Empty); // Fire event if amount changed!

        }
        public float GetResourceAmount(ResourceType resourceType) {
            return resourceAmountDictionary[resourceType];
        }
        /*
        public static event EventHandler OnResourceAmountChanged;
        //public event EventHandler OnResourceAmountChanged;
        //private float goldAmount;

        private static Dictionary<ResourceType, float> resourceAmountDictionary;
 
        public enum ResourceType {
            Iron,
            Wood,
        }
        public static void Init() {
            resourceAmountDictionary = new Dictionary<ResourceType, float>();

            foreach (ResourceType resourceType in System.Enum.GetValues(typeof(ResourceType))) { 
                resourceAmountDictionary[resourceType] = 0;// init all keys with 0
            }
        }

        public static void AddGoldAmount(float amount) {
            resourceAmountDictionary[ResourceType.Iron] += amount;
            OnResourceAmountChanged?.Invoke(null, EventArgs.Empty); // Fire event if amount changed!

        }
        public static float GetGoldAmount() {
            return resourceAmountDictionary[ResourceType.Iron];
        }
       
/*

        public  void AddResourceAmount(ResourceType resourceType, int amount) {
            resourceAmountDictionary[resourceType] += amount;
            if (OnResourceAmountChanged != null) OnResourceAmountChanged(null, EventArgs.Empty);
        }

        public  void RemoveResourceAmount(ResourceType resourceType, int amount) {
            resourceAmountDictionary[resourceType] -= amount;
            if (OnResourceAmountChanged != null) OnResourceAmountChanged(null, EventArgs.Empty);
        }

        public int GetResourceAmount(ResourceType resourceType) {
            return resourceAmountDictionary[resourceType];
        }
        */
        //------------------------------------------+ Resource Gatherer--------------------------------------------//

        #region Debug
        [Header("Options INFO")]
        [SerializeField] private float _MasterVolume;
        [SerializeField] private float _SFXVolume;
        [SerializeField] private float _MusicVolume;

        [Header("Player INFO")]
        [SerializeField] private string _PlayerName;
        [SerializeField] private float _PlayerShipHealth;
        [SerializeField] private float _PlayerShipMaxHealth;
        //Resources
        [SerializeField] private float _PlayerCredits;
        [SerializeField] private float _PlayerIronOre;

        [Header("AI INFO")]
        [SerializeField] private string _AIName;
        [SerializeField] private float _AICredits;
        [SerializeField] private float _AIHealth;

        [Header("Game INFO")]
        [SerializeField] private float _HomeDistance;
        #endregion

        private void Awake() {
            if (_instance != null) {//singleton
                Destroy(gameObject);
            } else {
                _instance = this; // global
            }
            _saveData = new SaveData(); //every game start create new Object to SaveData 
            _jsonSaver = new JSONSaver();
            Init(); //Init dic
        }

        private void Update() {
            #region Debug
            //Options
            _MasterVolume = MasterVolume;
            _SFXVolume = SFXVolume;
            _MusicVolume = MusicVolume;
            //Player
            _PlayerName = PlayerName;
            _PlayerShipHealth = PlayerShipHealth;
            _PlayerShipMaxHealth = PlayerShipMaxHealth;
            //res
            _PlayerCredits = PlayerCredits;
            _PlayerCredits = PlayerCredits;
            _PlayerIronOre = PlayerIronOre;
            // AI
            _AIName = AIName;
            _AICredits = AICredits;
            _AIHealth = AIHealth;
            //Game Info
            _HomeDistance = HomeDistance;
            // Planets
            #endregion

            if (Input.GetKeyDown(KeyCode.F9)) {
                ScreenCapture.CaptureScreenshot(Time.realtimeSinceStartup + "_HOME_Screenshot.jpeg");
            }
        }

        public readonly int _saveSlots = 3;


        public void Save() {
            _jsonSaver.Save(_saveData);
        }

        public void Load() {
            _jsonSaver.Load(_saveData);
        }
        public void DeleteData() {
            _jsonSaver.DeleteData();
        }
    }
}

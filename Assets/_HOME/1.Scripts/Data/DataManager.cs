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
        public readonly int _saveSlots = 3;

        //------------------------------------------Options--------------------------------------------//
        public float MasterVolume { get { return _saveData.masterVolume; } set { _saveData.masterVolume = value; } }
        public float SFXVolume { get { return _saveData.sfxVolume; } set { _saveData.sfxVolume = value; } }
        public float MusicVolume { get { return _saveData.musicVolume; } set { _saveData.musicVolume = value; } }
        //-----------------------------------------+Options--------------------------------------------//

        //------------------------------------------+PLayer--------------------------------------------//
        public string PlayerName { get { return _saveData.pName; } set { _saveData.pName = value; } }
        public float PlayerShipHealth { get { return _saveData.pShipHealth; } set { _saveData.pShipHealth = value; } }
        public float PlayerShipMaxHealth { get { return _saveData.pShipMaxHealth; } set { _saveData.pShipMaxHealth = value; } }
        //Resources
        public float PlayerCredits { get { return _saveData.pCredits; } set { _saveData.pCredits = value; } }
        public float PlayerIronOre { get { return _saveData.pIronOre; } set { _saveData.pIronOre = value; } }
        //------------------------------------------+PLayer--------------------------------------------//

        //------------------------------------------ AI --------------------------------------------//
        public int AiDifficultyIndex { get => _saveData.aiDifficulty; set => _saveData.aiDifficulty = value; }

        public string AIName { get { return _saveData.aiName; } set { _saveData.aiName = value; } }
        public float AICredits { get { return _saveData.aiCredits; } set { _saveData.aiCredits = value; } }
        public float AIHealth { get { return _saveData.aiHealth; } set { _saveData.aiHealth = value; } }
        //------------------------------------------+AI--------------------------------------------//

        //------------------------------------------ GameInfo--------------------------------------------//
        public float HomeDistance { get { return _saveData.homeDistance; } set { _saveData.homeDistance = value; } }
        //------------------------------------------+GameInfo--------------------------------------------//

        //------------------------------------------ Planets--------------------------------------------//
        public List<string> planetNames = new List<string> { "Oxan", "Thena", "Isa", "Venture", "Plexor", "Maximore", "Pic", "Xlas", "Juno", "Noctine", "Tropic", "Trine", "Waterborne", "Rustic", "Plant North", "Starlet" };

        [SerializeField] private List<PlanetSetupDefinition> _planetsVisited = new List<PlanetSetupDefinition>();
        public List<PlanetSetupDefinition> PlanetsVisited { get { return _planetsVisited; } set { _planetsVisited = value; } }
        //------------------------------------------ +Planets--------------------------------------------//

        //------------------------------------------ Resource Gatherer--------------------------------------------//
        public event EventHandler OnResourceAmountChanged;

        static public Dictionary<ResourceType, float> resourceAmountDictionary;

        public enum ResourceType {
            Empty,
            Iron,
            Alloy,
            Food,
            Energy,
            BauxiteOre,
            IronOre,
        }

        public static void InitResources(float resourceAmount) {
            resourceAmountDictionary = new Dictionary<ResourceType, float>();

            foreach (ResourceType resourceType in System.Enum.GetValues(typeof(ResourceType))) {
                resourceAmountDictionary[resourceType] = resourceAmount; // init all keys with 0
            }
        }

        public void AddResourceAmount(ResourceType resourceType, float amount) {
            resourceAmountDictionary[resourceType] += amount;
            OnResourceAmountChanged?.Invoke(null, EventArgs.Empty);
        }

        public float GetResourceAmount(ResourceType resourceType) {
            return resourceAmountDictionary[resourceType];
        }

        private void Awake() {
            if (_instance != null) {
                Destroy(gameObject);
            } else {
                _instance = this;
            }

            _saveData = new SaveData(); //every game start create new Object to SaveData 
            _jsonSaver = new JSONSaver();
            InitResources(0f); //Init dic
        }

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

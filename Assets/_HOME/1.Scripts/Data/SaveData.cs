using System.Collections;
using System.Collections.Generic;
using System;


namespace HOME.Data { //DEFAULT GAME DATA
    [Serializable]
    public class SaveData {
        //------------------------------------------Options--------------------------------------------//
        public float masterVolume;
        public float sfxVolume;
        public float musicVolume;

        //------------------------------------------+Options--------------------------------------------//
        //------------------------------------------ PLayerSetupDefinitions--------------------------------------------//
        public string pName;
        private readonly string defaultPName = "Player 1";
        public float pShipHealth;
        public float pShipMaxHealth;
        //resources
        public float pCredits;
        public float pIronOre;
        //------------------------------------------+PLayerSetupDefinitions--------------------------------------------//
        //------------------------------------------ AI--------------------------------------------//
        public int aiDifficulty;
        public string aiName;
        public float aiCredits;
        private readonly string defaultAIName = "AI";
        public float aiHealth;
        //------------------------------------------+AI--------------------------------------------//
        //------------------------------------------GameInfo--------------------------------------------//
        public float homeDistance;


        //------------------------------------------+GameInfo--------------------------------------------//
        //------------------------------------------ Planets--------------------------------------------//
        //Planet1
        //public List<PlanetSetupDefinition> planetsVisited = new List<PlanetSetupDefinition>();
        //------------------------------------------+Planets--------------------------------------------//
        public SaveData() { //public constructor //awake
            //------------------------------------------Options--------------------------------------------//
            masterVolume = 0f;
            sfxVolume = 0f;
            musicVolume = 0f;
            //-----------------------------------------+Options--------------------------------------------//
            //------------------------------------------+PLayerSetupDefinitions--------------------------------------------//
            pName = defaultPName;
            pShipHealth = 400f;
            pShipMaxHealth = 500f;
            //resources
            pCredits = 0f;
            pIronOre = 0f;
            //------------------------------------------+PLayerSetupDefinitions--------------------------------------------//
            //------------------------------------------ AI--------------------------------------------//
            aiDifficulty = 1;
            aiName = defaultAIName;
            aiCredits = 0f;
            aiHealth = 100f; ;
            //------------------------------------------+AI--------------------------------------------//
            //------------------------------------------GameInfo--------------------------------------------//
            homeDistance = 100f;
            //------------------------------------------+GameInfo--------------------------------------------//
            //------------------------------------------ Planets--------------------------------------------//
            
            //------------------------------------------+Planets--------------------------------------------//
        }
    }
}
using UnityEngine;

namespace HOME.Game {

    [System.Serializable]
    public class PlanetSetupDefinition {

        public int planetID; // planet number
        public string planetInfo;
        public float planetDistanceToShip;
        public float planetDamageToShip;
        public Transform planetTransform;
        public bool planetSelected;

        public PlanetSetupDefinition(int pNr, string pInfo, float pDistanceTS, float pDamageTS, Transform pobj, bool pPicked) {
            //public PlanetSetupDefinition(int pNr, string pInfo, float pDistanceTS, float pDamageTS, Transform pTransform, bool pPicked) {
            planetID = pNr;
            planetInfo = pInfo;
            planetDistanceToShip = pDistanceTS;
            planetDamageToShip = pDamageTS;
            planetTransform = pobj;
            planetSelected = pPicked;
        }
    }
}

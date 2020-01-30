using System.Collections.Generic;
using UnityEngine;

namespace HOME.Game {

    [System.Serializable]
    public class PlayerSetupDefinition {

        public string playerName;
        public int playerID;
        public Transform playerLocation;
        public Color playerColor;
        public bool isAi;
        public float playerCredits; // AI uses this
        public List<GameObject> StartingEntitys = new List<GameObject>();

        [SerializeField] private List<GameObject> _activeUnits = new List<GameObject>();
        public List<GameObject> ActiveUnits { get { return _activeUnits; } set { _activeUnits = value; } }
    }
}

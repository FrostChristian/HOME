using UnityEngine;
using System.Collections.Generic;

namespace HOME.Game {

    public class AISupport : MonoBehaviour {

        public List<GameObject> AIUnits = new List<GameObject>(); //manage drones
        public List<GameObject> AIBases = new List<GameObject>(); //manage Buildings

        public PlayerSetupDefinition Player = null; // to store the info for the AI in Player

        public static AISupport GetSupport(GameObject go) {
            return go.GetComponent<AISupport>();
        }

        public void Refresh() {
            AIUnits.Clear();
            AIBases.Clear();
            foreach (var u in Player.ActiveUnits) {
                if (u.name.Contains("Tank")) AIUnits.Add(u);
                if (u.name.Contains("AIBase")) AIBases.Add(u);
            }
        }
    }
}

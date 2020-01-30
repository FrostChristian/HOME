using UnityEngine;

namespace HOME.Game {

    public class CreateBaseAI : AIBehavior {

        private AISupport support = null;
        public float cost = 0f; //Building Cost
        public int unitsPerBase = 5; //how many units can a base support
        public float buildDistance = 30f;
        public int triesPerDrone = 3;
        public GameObject basePrefab;
        private GameManager gameManager;

        private void Start() {
            gameManager = FindObjectOfType<GameManager>();
        }

        public override float GetWeight() {
            if (support == null) {
                support = AISupport.GetSupport(gameObject);
            }
            if (support.Player.playerCredits < cost || support.AIUnits.Count == 0) { //check for money
                return 0;
            }
            if (support.AIBases.Count * unitsPerBase <= support.AIUnits.Count) {
                return 1;
            }
            return 0;
        }

        public override void Execute() {

            if (support.Player.playerCredits < cost) {
                return;
            } 

            var go = Instantiate(basePrefab); // base
            go.GetComponent<Player>().Info = support.Player; // set player info

            foreach (var drone in support.AIUnits) {
                for (int i = 0; i < triesPerDrone; i++) {
                    var pos = drone.transform.position; // start at drones position
                    pos += Random.insideUnitSphere * buildDistance; // change pos to something random (insideUnitSphere is between -1/1) * build distance para
                    pos.y = Terrain.activeTerrain.SampleHeight(pos); // get terrain height and set the building height to it
                    go.transform.position = pos;

                    if (gameManager.IsEntitySafeToPlace(go)) {
                        support.Player.playerCredits -= cost;
                        return;
                    }
                }
            }
            Destroy(go);
        }
    }
}

using UnityEngine;

namespace HOME.Game {

    public class AttackAI : AIBehavior {

        public int dronesRequired = 10;
        public float attackSquadSize = 0.5f; // 1 = 100% size
        public float timeDelay = 5;
        public int increasedPerWave = 10;
        private GameManager gameManager;

        private void Start() {
            gameManager = FindObjectOfType<GameManager>();
        }

        public override void Execute() {
            var ai = AISupport.GetSupport(this.gameObject); // get AI and pass in this
            //Debug.Log(ai.Player.playerName + " is attacking");

            int wave = (int)(ai.AIUnits.Count * attackSquadSize); // current wave size
            dronesRequired += increasedPerWave; // increse drones required

            foreach (var p in gameManager.Factions) { //loop through players to find the human player
                if (p.isAi) { // filter out AI
                    continue;
                }
                for (int i = 0; i < wave; i++) {
                    var drone = ai.AIUnits[i]; //get drone
                    var nav = drone.GetComponent<Unit>(); // get navagent
                    nav.SendAIToTarget(p.playerLocation.position, () => { Debug.Log("Attackig!"); }); //send to player
                }
                return;
            }
        }

        public override float GetWeight() {
            if (TimePassed < timeDelay)
                return 0;
            TimePassed = 0;

            var ai = AISupport.GetSupport(this.gameObject);
            if (ai.AIUnits.Count < dronesRequired)
                return 0;

            return 1;
        }
    }
}
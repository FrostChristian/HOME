using UnityEngine;
using System.Collections.Generic;

namespace HOME.Game {

    public class AIController : MonoBehaviour {

        public string playerName = "AI"; // stringreference !!!
        public float confusion = 0.3f;
        public float frequency = 1f;
        public float waited = 1f;

        private PlayerSetupDefinition _aIPlayers;
        public PlayerSetupDefinition Player { get { return _aIPlayers; } }

        private GameManager gameManager;
        private List<AIBehavior> AIs = new List<AIBehavior>();
        private bool _loaded;

        private void Start() {
            gameManager = FindObjectOfType<GameManager>();
            foreach (var ai in GetComponents<AIBehavior>()) {
                AIs.Add(ai);
            }
            foreach (var player in gameManager.activePlayers) {
                if (player.playerName == playerName) {

                    _aIPlayers = player;
                }
            }
            //Implement AI support
            //Set Player info
            gameObject.AddComponent<AISupport>().Player = _aIPlayers;
        }

        void Update() {
            waited += Time.deltaTime;
            if (waited < frequency) {
                return;
            }
            string aiLog = ""; //debugging
            float bestAiValue = float.MinValue;
            AIBehavior bestAi = null;

            //get new information for decicions
            if (AISupport.GetSupport(gameObject) == null) {  //null check!
                return;
            }
            AISupport.GetSupport(gameObject).Refresh();

            foreach (var ai in AIs) {
                ai.TimePassed += waited;
                var aiValue = ai.GetWeight() * ai.weightMultiplier + Random.Range(0, confusion);
                aiLog += ai.GetType().Name + ": " + aiValue + ";  "; //debugging
                if (aiValue > bestAiValue) {
                    bestAiValue = aiValue;
                    bestAi = ai;
                }
            }
            bestAi.Execute();
            waited = 0;
        }
    }
}
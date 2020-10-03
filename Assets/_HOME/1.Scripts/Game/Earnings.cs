using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HOME.Data;

namespace HOME.Game {

    public class Earnings : MonoBehaviour {
        [Header("Income/Cost")]
        [SerializeField] public float CreditsPerSecond = 10f;
        public PlayerSetupDefinition player; // to store the info for the AI in Player

        private void Start() {
            player = GetComponent<Player>().Info;
        }

        void Update() {
            if (player.isAi) {
                player.playerCredits += CreditsPerSecond;
            }

            if (!player.isAi) {
                player.playerCredits += CreditsPerSecond;
                DataManager.Instance.PlayerCredits = player.playerCredits;
            }
        }
    }
}

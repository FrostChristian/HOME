using System.Collections.Generic;
using UnityEngine;

namespace HOME.Game {

    [CreateAssetMenu(fileName = "Difficultys", menuName = "Difficulty/Create Difficulty", order = 1)]
    public class DifficultySettings : ScriptableObject { // ability to create Scriptable obj in inspector

        [SerializeField] private List<GameDifficultyDefinition> _difficulty;
        public int TotalDifficultys { get { return _difficulty.Count; } }
        public GameDifficultyDefinition GetDifficulty(int index) {
            return _difficulty[index];
        }
    }
}
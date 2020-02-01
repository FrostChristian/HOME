using UnityEngine;

namespace HOME.Game {
    // handles selecting an item from a wrap around list
    public class DifficultyManager : MonoBehaviour {

        [SerializeField] protected DifficultySettings _difficultySettings = default;
        [SerializeField] private int _currentIndex = 0;
        public int CurrentIndex { get { return _currentIndex; } }

        public void ClampIndex() {
            if (_difficultySettings.TotalDifficultys == 0) {
                Debug.LogWarning("DifficultySelector ClampIndex: missing difficulty setup!");
                return;
            }

            if (_currentIndex >= _difficultySettings.TotalDifficultys) {
                _currentIndex = 0;
            }

            if (_currentIndex < 0) {
                _currentIndex = _difficultySettings.TotalDifficultys - 1;
            }
        }

        public void SetIndex(int index) {
            _currentIndex = index;


            ClampIndex();
        }

        public void IncrementIndex() {
            _currentIndex++;
            Debug.Log("OI inc");
            ClampIndex();
        }

        public void DecrementIndex() {
            _currentIndex--;
            ClampIndex();
        }

        public GameDifficultyDefinition GetDifficulty(int index) { // return mission specs based on an index
            return _difficultySettings.GetDifficulty(index);
        }

        public GameDifficultyDefinition GetCurrentDifficulty() { // return mission specs for current index
            return _difficultySettings.GetDifficulty(_currentIndex);
        }
    }
}
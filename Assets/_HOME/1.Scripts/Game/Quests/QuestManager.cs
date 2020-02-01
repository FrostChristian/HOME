using HOME.UI;
using UnityEngine;

namespace HOME.Game {

    public class QuestManager : MonoBehaviour {    // handles selecting an item from a wrap around list

        private static QuestManager _instance;
        public static QuestManager Instance { get => _instance; }

        [SerializeField] protected QuestSettings _questSettings = default;
        [SerializeField] private int _currentIndex = 0;
        public int CurrentIndex { get { return _currentIndex; } }

        private void Awake() {
            if (_instance != null) {//singleton
                Destroy(gameObject);
            } else {
                _instance = this; 
            }
            //SetIndex(4);
        }
      

        public void ClampIndex() {
            if (_questSettings.TotalQuests == 0) {
                Debug.LogWarning("QuestManager ClampIndex: missing quest setup!");
                return;
            }

            if (_currentIndex >= _questSettings.TotalQuests) {
                _currentIndex = 0;
            }

            if (_currentIndex < 0) {
                _currentIndex = _questSettings.TotalQuests - 1;
            }
        }
   
        public void SetIndex(int index) {
            _currentIndex = index;
            //Debug.Log("Quest Idex == " + index);

            ClampIndex();
        }

        public void IncrementIndex() {
            _currentIndex++;
            ClampIndex();
        }

        public void DecrementIndex() {
            _currentIndex--;
            ClampIndex();
        }

        public QuestSetupDefinition GetQuest(int index) { // return mission specs based on an index
            return _questSettings.GetQuest(index);
        }

        public QuestSetupDefinition GetCurrentQuest() { // return mission specs for current index
            return _questSettings.GetQuest(_currentIndex);
        }
    }
}
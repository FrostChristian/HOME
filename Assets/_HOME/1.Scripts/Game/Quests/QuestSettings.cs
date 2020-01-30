using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOME.Game {

    [CreateAssetMenu(fileName = "Quest", menuName = "Quests/Create Quest", order = 1)]

    public class QuestSettings : ScriptableObject {

        [SerializeField] private List<QuestSetupDefinition> _quest;

        public int TotalQuests { get { return _quest.Count; } }

        public QuestSetupDefinition GetQuest(int index) {
            return _quest[index];
        }

    }

}
using System;
using UnityEngine;

namespace HOME.Game {

    [Serializable]
    public class GameDifficultyDefinition {

        [SerializeField] private string _name; // used in menu        
        [SerializeField] [Multiline] private string _description; // general details        
        [SerializeField] private string _sceneName; // scene name for loading        
        [SerializeField] private int _id; // unique identifier for save data        
        [SerializeField] private Sprite _image; // image used for menu
        [SerializeField] private float _healthModifier; // image used for menu
        [SerializeField] private float _dmgModifier; // image used for menu

        public string Name { get { return _name; } }
        public string Description { get { return _description; } }
        public string SceneName { get { return _sceneName; } }
        public int Id { get { return _id; } }
        public Sprite Image { get { return _image; } }
        public float HealthModifier { get { return _healthModifier; } }
        public float DmgModifier { get { return _dmgModifier; } }
    }
}
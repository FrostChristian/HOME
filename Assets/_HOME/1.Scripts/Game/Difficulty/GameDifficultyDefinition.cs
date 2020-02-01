using System;
using UnityEngine;

namespace HOME.Game {

    [Serializable]
    public class GameDifficultyDefinition {

        [SerializeField] private string _name = default; // used in menu        
        [SerializeField] [Multiline] private string _description = default; // general details        
        [SerializeField] private string _sceneName = default; // scene name for loading        
        [SerializeField] private int _id = default; // unique identifier for save data        
        [SerializeField] private Sprite _image = default; // image used for menu
        [SerializeField] private float _healthModifier = default; // image used for menu
        [SerializeField] private float _dmgModifier = default; // image used for menu

        public string Name { get { return _name; } }
        public string Description { get { return _description; } }
        public string SceneName { get { return _sceneName; } }
        public int Id { get { return _id; } }
        public Sprite Image { get { return _image; } }
        public float HealthModifier { get { return _healthModifier; } }
        public float DmgModifier { get { return _dmgModifier; } }
    }
}
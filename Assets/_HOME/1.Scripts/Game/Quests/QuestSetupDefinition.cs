using System;
using UnityEngine;

namespace HOME.Game {

    [Serializable]
    public class QuestSetupDefinition {

        [SerializeField] private string _name; // used in menu
        [SerializeField] private int _id; // uniqe id
        [SerializeField] private Sprite _icon; // image used for menu
        [SerializeField] [Multiline] private string _descriptionLong; // Long details        
        [SerializeField] [Multiline] private string _descriptionShort; // general details        
        [SerializeField] private int _conditionCount; // how often do we have to do this       
        [SerializeField] private int _progress; // progress ref      
        
        public string Name { get { return _name; } }
        public int Id { get { return _id; } }
        public Sprite Icon { get { return _icon; } }
        public string DescriptionLong { get { return _descriptionLong; } }
        public string DescriptionShort { get { return _descriptionShort; } }
        public int Condition { get { return _conditionCount; } }
        public int Progress { get { return _progress; } set { _progress = value; } }
    }
}
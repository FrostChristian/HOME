using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace HOME.Game {

    public abstract class ActionBehavior : MonoBehaviour {

        [HideInInspector] public abstract Action GetClickAction();
        [HideInInspector] public Entity _prefabEntity;
        public Sprite icon;
    }
}
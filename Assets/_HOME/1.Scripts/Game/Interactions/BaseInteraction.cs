using UnityEngine;

namespace HOME.Game {

    public abstract class BaseInteraction : MonoBehaviour { // select arcitecture
        public abstract void Select();
        public abstract void Deselect();

    }
}
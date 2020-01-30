using UnityEngine;

namespace HOME.Game {

    public abstract class AIBehavior : MonoBehaviour {

        public float weightMultiplier = 1f;
        public float TimePassed = 0f;

        public abstract float GetWeight();
        public abstract void Execute();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HOME.Game {

    public class DontDestroyOnLoad : MonoBehaviour {
        private void Awake() {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject); //only works for root level
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOME.Game {

    public class Interactive : MonoBehaviour {
        private bool _Selected = false; //already selected?
        public bool Selected { get { return _Selected; } }

        public bool SelectDeselect = false;

        public void Select() {
            _Selected = true;
            foreach (BaseInteraction selection in GetComponents<BaseInteraction>()) { // On unit selection, get all 'interactionsXYZ : BaseInteraction' and call their select() methods
                selection.Select();
            }
        }

        public void Deselect() {
            _Selected = false;
            foreach (BaseInteraction selection in GetComponents<BaseInteraction>()) {
                if (selection.gameObject == null) {
                    Debug.Log("Interactive Deselect(): Selection Destroyed");
                    continue;
                } else {

                    selection.Deselect();
                }
            }
        }

        void Update() {
            if (SelectDeselect) {
                SelectDeselect = false; // set to false
                if (_Selected) { // if selected >deselect
                    Deselect();
                } else { // if not selected >select
                    Select();
                }
            }
        }
    }
}

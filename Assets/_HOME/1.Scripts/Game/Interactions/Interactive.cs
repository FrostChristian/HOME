using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOME.Game {

    public class Interactive : MonoBehaviour {
        private bool _Selected = false;
        public bool Selected { get { return _Selected; } }

        public bool SelectDeselect = false;

        public void Select() {
            _Selected = true;
            foreach (BaseInteraction selection in GetComponents<BaseInteraction>()) {
                selection.Select();
            }
        }

        public void Deselect() {
            _Selected = false;
            foreach (BaseInteraction selection in GetComponents<BaseInteraction>()) {
                if (selection.gameObject == null) {
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

using HOME.UI;
using UnityEngine;

namespace HOME.Game {

    [RequireComponent(typeof(Interactive))]
    public class ShowActionButton : BaseInteraction {

        private InGameMenu _inGameMenu;

        private void Start() {
            _inGameMenu = FindObjectOfType<InGameMenu>();
        }

        public override void Deselect() {
            _inGameMenu.ClearButtons();
        }
        public override void Select() {
            _inGameMenu.ClearButtons();
            foreach (var ab in GetComponents<ActionBehavior>()) {
                _inGameMenu.AddButton(
                    ab.icon,
                    ab.GetClickAction(),
                    ab._prefabEntity);
            }
        }
    }
}

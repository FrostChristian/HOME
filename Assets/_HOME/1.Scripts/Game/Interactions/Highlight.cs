using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOME.Game {

    [RequireComponent(typeof(Interactive))]

    public class Highlight : BaseInteraction {

        private SpriteRenderer _rend;
        private PlayerSetupDefinition _player;
        private Color Red = new Color(1f, 0f, 0f);
        private Color Green = new Color(0f, 255f, 122f);
        public GameObject DisplayItem;

        public virtual void Awake() {
            if (DisplayItem == null) {
                Destroy(this);
                return;
            }
            _rend = DisplayItem.GetComponent<SpriteRenderer>();
            if (GetComponent<Player>() != null) {
                _player = GetComponent<Player>().Info;
            }
        }

        void Start() {
            if (_player != null && _player.isAi) {
                _rend.color = Red;
            } else {
                _rend.color = Green;
            }

            DisplayItem.SetActive(false);
        }

        public override void Deselect() {
            DisplayItem.SetActive(false);
        }

        public override void Select() {
            DisplayItem.SetActive(true);
        }
    }
}

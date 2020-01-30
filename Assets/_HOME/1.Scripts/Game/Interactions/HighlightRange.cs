using UnityEngine;

namespace HOME.Game {

    [RequireComponent(typeof(Interactive))]
    public class HighlightRange : Highlight {

        [SerializeField] public Projector _rangeProjector;
        [SerializeField] private bool isEnabled = true;

        public override void Awake() {
            base.Awake();
            if (_rangeProjector == null) {
                Destroy(this);
                Debug.Log("RangeDisplay Awake No Projector! " + name);
                return;
            }
            GameManager.playerRangeProjectors.Add(_rangeProjector);
        }

        public override void Deselect() {
            base.Deselect();
            _rangeProjector.enabled = false;
        }

        public override void Select() {
            base.Select();
            _rangeProjector.enabled = true;
        }

        private void OnDestroy() {
            GameManager.playerRangeProjectors.Remove(_rangeProjector);
        }
    }
}

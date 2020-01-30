using UnityEngine;
using UnityEngine.UI;

namespace HOME.Game {

    public class LoadingBar : MonoBehaviour {
        // simple script to update loading progress

        [SerializeField] private Slider _slider;    
        private float _targetProgressValue;    
        public float sliderValue;    
        private const float paddingValue = 0.15f;    // asynchronous load ends at 0.1f, pad the value so the bar fills out correctly
        private const float _lerpSpeed = 0.3f;    // speed to animate progress bar


        private void Start() {
            if (_slider == null) {
                _slider = gameObject.GetComponentInChildren<Slider>();
            }
            InitSlider();
        }

        public void UpdateProgress(float progressValue) {
            if (_slider != null) {
                _targetProgressValue = progressValue + paddingValue;
            }
        }

        public void InitSlider() {
            sliderValue = 0f;
        }
        
        private void Update() {
            if (_slider != null) {
                if (Mathf.Abs(_slider.value - _targetProgressValue) > .01f) {
                    _slider.value = Mathf.Lerp(_slider.value, _targetProgressValue, _lerpSpeed);//lerp to show some progress bar movement
                    sliderValue = _slider.value;
                }
            }
        }
    }
}

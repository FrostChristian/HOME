using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using HOME.UI;
using System;

namespace HOME.Game {
    // handles UI events
    public class UIEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {// required interface when using the OnPointerEnter method.

        public Action MouseOverOnceTooltipFunc = null;
        public Action MouseOutOnceTooltipFunc = null;

        public Button theButton;
        public Slider theSlider;
        private float timeCount;

        public AudioClip _mouseHover;
        public AudioClip _mouseClick;
        public AudioClip _mouseDrag;
        private AudioSource _source;
        private float _delay = 0.05f;

        private void Start() {
            _source = gameObject.GetComponentInParent<AudioSource>(); //.GetComponent<AudioSource>();
            if (_source == null) {
                _source = gameObject.AddComponent<AudioSource>();
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            _source.PlayOneShot(MenuManager.Instance.mouseHover);
            MouseOverOnceTooltipFunc?.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData) {
            MenuManager.Instance.PlayClickSound();
        }

        public void OnPointerExit(PointerEventData eventData) {
            MouseOutOnceTooltipFunc?.Invoke();
        }

        public void OnBeginDrag(PointerEventData data) {
        }

        public void OnDrag(PointerEventData data) {
            if (data.dragging) {
                timeCount += Time.deltaTime;
                if (timeCount > 1.0f) {
                    _source.PlayOneShot(MenuManager.Instance.mouseDrag);
                    Debug.Log("Dragging:" + data.position);
                    timeCount = 0.0f;
                }
            }
        }

        public void OnEndDrag(PointerEventData data) {
            Debug.Log("OnEndDrag: " + data.position);
        }

        public void Destroy() {
            Destroy(this);
        }
    }
}




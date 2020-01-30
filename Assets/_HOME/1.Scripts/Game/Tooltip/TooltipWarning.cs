/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HOME.Game {

    public class TooltipWarning : MonoBehaviour {

        private static TooltipWarning instance;

        private Camera uiCamera;
        [SerializeField]
        private RectTransform canvasRectTransform;

        [SerializeField] private Text tooltipText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private RectTransform backgroundRectTransform;
        private Func<string> getTooltipStringFunc;
        private float showTimer;
        private float flashTimer;
        private int flashState;

        private void Awake() {
            instance = this;
            //backgroundRectTransform = transform.Find("background").GetComponent<RectTransform>();
            //tooltipText = transform.Find("text").GetComponent<Text>();
            //backgroundImage = transform.Find("background").GetComponent<Image>();

            HideTooltip();
        }

        private void Update() {

            Vector2 localPoint = Input.mousePosition;
            SetText(getTooltipStringFunc());
            if (Screen.width - Input.mousePosition.x < backgroundRectTransform.rect.width) {

                localPoint -= new Vector2(backgroundRectTransform.rect.width, 0);
            }

            if (Screen.height - Input.mousePosition.y < backgroundRectTransform.rect.height) {

                localPoint -= new Vector2(0, backgroundRectTransform.rect.height);
            }

            transform.position = localPoint;

            flashTimer += Time.deltaTime;
            float flashTimerMax = .033f;
            if (flashTimer > flashTimerMax) {
                flashTimer = 0f;
                flashState++;
                switch (flashState) {
                    case 1:
                    case 3:
                    case 5:
                    tooltipText.color = new Color(1, 1, 1, 1);
                    backgroundImage.color = new Color(178f / 255f, 0 / 255f, 0 / 255f, 1);
                    break;
                    case 2:
                    case 4:
                    tooltipText.color = new Color(178f / 255f, 0 / 255f, 0 / 255f, 1);
                    backgroundImage.color = new Color(1, 1, 1, 1);
                    break;
                }
            }

            showTimer -= Time.deltaTime;
            if (showTimer <= 0f) {
                HideTooltip();
            }
        }

        private void ShowTooltip(string tooltipString, float showTimerMax = .5f) {
            ShowTooltip(() => tooltipString, showTimerMax);
        }

        private void ShowTooltip(Func<string> getTooltipStringFunc, float showTimerMax = .5f) {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            this.getTooltipStringFunc = getTooltipStringFunc;
            showTimer = showTimerMax;
            flashTimer = 0f;
            flashState = 0;
            Update();
        }

        private void SetText(string tooltipString) {
            tooltipText.text = tooltipString;
            //float textPaddingSize = 5f;
            //Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + textPaddingSize * 4f, tooltipText.preferredHeight + textPaddingSize * 2f);
            //backgroundRectTransform.sizeDelta = backgroundSize;
        }

        private void HideTooltip() {
            gameObject.SetActive(false);
        }

        public static void ShowTooltip_Static(string tooltipString) {
            instance.ShowTooltip(tooltipString);
        }

        public static void ShowTooltip_Static(Func<string> getTooltipStringFunc) {
            instance.ShowTooltip(getTooltipStringFunc);
        }

        public static void HideTooltip_Static() {
            instance.HideTooltip();
        }
    }
}

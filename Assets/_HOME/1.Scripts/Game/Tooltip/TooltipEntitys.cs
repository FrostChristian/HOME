/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */
 /// 
 /// Modified using screenspace instead of uiCamera
 /// 

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HOME.Game {

    public class TooltipEntitys : MonoBehaviour {

        private static TooltipEntitys instance;


        private Camera uiCamera;
        [SerializeField]
        private RectTransform canvasRectTransform;

        [SerializeField] private Image _entityIcon;
        [SerializeField] private Text _entityName;
        [SerializeField] private Text _entityDescription;
        [SerializeField] private Text _entityIronCost;
        [SerializeField] private Text _entityFoodCost;
        [SerializeField] private Text _entityAlloyCost;
        [SerializeField] private Text _entityEnergyCost;
        [SerializeField] private RectTransform backgroundRectTransform;

        private void Awake() {
            instance = this;
            /*   backgroundRectTransform = transform.Find("background").GetComponent<RectTransform>();
                 entityImage = transform.Find("image").GetComponent<Image>();
                 entityName = transform.Find("nameText").GetComponent<Text>();
                 entityDescription = transform.Find("descriptionText").GetComponent<Text>();
                 strText = transform.Find("strText").GetComponent<Text>();
                 conText = transform.Find("conText").GetComponent<Text>();
                 entityIronCost = transform.Find("dexText").GetComponent<Text>();
         */
            HideTooltip();
        }

        private void Update() {
            /*    Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, uiCamera, out localPoint);
                transform.localPosition = localPoint;

                Vector2 anchoredPosition = transform.GetComponent<RectTransform>().anchoredPosition;
                if (anchoredPosition.x + backgroundRectTransform.rect.width > canvasRectTransform.rect.width) {
                    anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width;
                }
                if (anchoredPosition.y - backgroundRectTransform.rect.height > canvasRectTransform.rect.height) {
                    anchoredPosition.y = canvasRectTransform.rect.height + backgroundRectTransform.rect.height;
                }
                transform.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;*/

            Vector2 localPoint = Input.mousePosition;

            if (Screen.width - Input.mousePosition.x < backgroundRectTransform.rect.width) {

                localPoint -= new Vector2(backgroundRectTransform.rect.width, 0);
            }

            if (Screen.height - Input.mousePosition.y < backgroundRectTransform.rect.height) {

                localPoint -= new Vector2(0, backgroundRectTransform.rect.height);
            }

            transform.position = localPoint;

            if (Input.GetMouseButtonDown(0)) {
                //HideTooltip();
            }
        }

        private void ShowTooltip(Sprite entityIcon, string entityName, string entityDescription, float iron, float food, float alloy, float energy) {
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
            _entityIcon.sprite = entityIcon;
            _entityName.text = entityName;
            _entityDescription.text = entityDescription;
            _entityIronCost.text = iron.ToString();
            _entityFoodCost.text = food.ToString();
            _entityAlloyCost.text = alloy.ToString();
            _entityEnergyCost.text = energy.ToString();
            Update();
        }

        private void HideTooltip() {
            Debug.Log("HideTooltip");
            gameObject.SetActive(false);
        }

        public static void ShowTooltip_Static(Sprite entityIcon, string entityName, string entityDescription, float iron, float food, float alloy, float energy) {
            instance.ShowTooltip(entityIcon, entityName, entityDescription, iron, food, alloy, energy);
        }

        public static void HideTooltip_Static() {
            instance.HideTooltip();
        }

        public static void AddTooltip(Transform targTransform, Sprite entityIcon, string entityName, string entityDescription, float iron, float food, float alloy, float energy) {
            if (targTransform.GetComponentInChildren<UIEvents>(true) != null) {
                targTransform.GetComponentInChildren<UIEvents>(true).MouseOverOnceTooltipFunc = () => TooltipEntitys.ShowTooltip_Static(entityIcon, entityName, entityDescription, iron, food, alloy, energy);
                targTransform.GetComponentInChildren<UIEvents>(true).MouseOutOnceTooltipFunc = () => TooltipEntitys.HideTooltip_Static();
            }

        }
    }
}


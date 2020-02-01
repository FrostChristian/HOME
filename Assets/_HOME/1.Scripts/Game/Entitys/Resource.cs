using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using HOME.Data;

namespace HOME.Game {

    public class Resource : Entity {

        public static event EventHandler OnResourceEntityClicked;

        [SerializeField] public DataManager.ResourceType resourceType = default; // assign in inspector

        [SerializeField] private float resourceGenerationAmount = 0f;
        public Transform resourceEntityCollectPointTransform;
        public List<GameObject> crates = new List<GameObject>();

        public override void Awake() {
            base.Awake();
            Assert.IsFalse(resourceType == DataManager.ResourceType.Empty, "Resource Awake(): Resource Type needs to be assigned + must not be Empty!");
            if (tag == "Mine") {
                ResourceManager.Instance.resourceEntityList.Add(this);
            }
            if (resourceEntityCollectPointTransform == null) { // if no coll point assigned use own transform
                resourceEntityCollectPointTransform = transform;
            }

            ClickFunc = () => {         //if i clicked this Resi fire event
                OnResourceEntityClicked?.Invoke(this, EventArgs.Empty);
            }; // send this to subscriber
        }

        public override void Update() {
            base.Update();
            if (_currInventoryAmount < _maxInventoryAmount) { // inventory full? is regenerating resources?
                if (resourceGenerationAmount != 0f) { // is generation set?

                    DataManager.Instance?.AddResourceAmount(resourceType, resourceGenerationAmount); // add it directy to the game!

                    _currInventoryAmount += resourceGenerationAmount; // regenerate resources
                }
                HandleCrates(); // change look of entity
            } else {
                _currInventoryAmount = _maxInventoryAmount; // set to max
            }
        }

        public Vector3 GetPosition() {
            return resourceEntityCollectPointTransform.position;
        }

        public DataManager.ResourceType GrabResourceType(float amountGrabbed, out float actualAmountTaken) {
            if (_currInventoryAmount < amountGrabbed) {// if grabamount is bigger than the actual inventory amount
                amountGrabbed = _currInventoryAmount; // take the current inventory amount
            }
            actualAmountTaken = amountGrabbed; // out actualAmountTaken set to maximum amount available
            _currInventoryAmount -= amountGrabbed; // grab amount from this recource node
            return resourceType;
        }

        public bool HasResources() {
            return _currInventoryAmount > 0;
        }

        public DataManager.ResourceType GetResourceType() {
            return resourceType;
        }


        private void HandleCrates() { // for optical representation of resurces
            if (crates.Count > 0) { // if crates available
                float amoutPerCrate = _maxInventoryAmount / crates.Count;
                for (int i = 0; i < crates.Count; i++) {
                    if (_currInventoryAmount > amoutPerCrate * i) {
                        crates[i].SetActive(true);
                    } else {
                        crates[i].SetActive(false);
                    }
                }
            }
        }

        public override void InGameMenuUpdate() { // TODO make better!
            base.InGameMenuUpdate();
            float healthBar = CurrHealth / MaxHealth;
            float resourceBar = _currInventoryAmount / _maxInventoryAmount;
            _inGameMenu.SelectionFill(
                GetName,
                hasHealth, // has health ?
                CurrHealth + "/" + MaxHealth,
                GetDescription,
                GetIcon,
                player.isAi,
                healthBar,
                hasResources,
                "Available Resources: " + resourceType,
                resourceBar,
                _currInventoryAmount.ToString() + "/" + _maxInventoryAmount.ToString());
        }
    }
}
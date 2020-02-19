using System;
using System.Collections.Generic;
using UnityEngine;
using HOME.Data;

namespace HOME.Game {

    public class GatherAI : MonoBehaviour {

        private enum State {
            NotGathering,
            MoveToResourceEntity,
            CollectingResources,
            MoveToStorage,
        }

        [Header("GatherAI")]
        [SerializeField] private float intakeCapacity = 1f;
        [Space]
        [SerializeField] private State _state = default;
        [SerializeField] private Transform _storageTransform = default;
        [SerializeField] private TextMesh _inventoryTextMesh = default;

        public Resource _resourceEntity;
        public DataManager.ResourceType _iCarry;
        [SerializeField] private Dictionary<DataManager.ResourceType, float> _inventoryAmountDicctonary = default;
        private Unit _thisUnit;
        private DataManager.ResourceType _initialResourcetype = DataManager.ResourceType.Empty;
        private Action onArrivedAtPosition;

        public void Init(Unit unit) {
            _thisUnit = unit;
            // get first resource type!
            _initialResourcetype = _thisUnit.madeBy.GetComponent<Entity>().takesResourceType;
            //set the nearest resource type to target
            _resourceEntity = ResourceManager.GetResourceEntityNearPosition_Static(transform.position, _initialResourcetype);
        }


        private void Awake() {
            _state = State.NotGathering;

            _inventoryAmountDicctonary = new Dictionary<DataManager.ResourceType, float>(); // init the inventory
            foreach (DataManager.ResourceType resourceType in System.Enum.GetValues(typeof(DataManager.ResourceType))) {
                _inventoryAmountDicctonary[resourceType] = 0;
            }
        }

        private void Start() {
            if (DataManager.Instance == null) {
                Debug.Log("No Datamanager Found ERROR");
                return;
            }
            if (intakeCapacity <= 0f) {
                Debug.Log("GatherAI Update() This gatherer has no means to gather!");
                _state = State.NotGathering;
                return;
            }
        }

        private void Update() {
            //Debug.Log("Gather AI: State:  " + _state);

            switch (_state) {
                case State.NotGathering:
                    //_resourceEntity = ResourceManager.GetResourceEntity_Static();
                    if (_resourceEntity != null) { //if resourceEntity available, try to move towards it
                        _state = State.MoveToResourceEntity;
                    }
                    break;

                case State.MoveToResourceEntity:
                    if (_thisUnit.IsIdle()) { // are we doing anything more important?
                        _thisUnit.SendAIToTarget(_resourceEntity.GetPosition(), () => { // move to resourceEntity set Action to call next state
                            _state = State.CollectingResources;
                        });
                    }
                    break;

                case State.CollectingResources:
                    if (_thisUnit.IsIdle() && _resourceEntity != null) { // if we can gather and have a resource
                        if (IsInventoryFull() && _iCarry != _resourceEntity.GetResourceType()) { // if we have a full inventory and got sent to a different resource
                            DropInventory(); // drop inv
                        }
                        if ((IsInventoryFull() || !_resourceEntity.HasResources())) { //if we have a full inventory or the entity has no resources
                                                                                      //Move to Storage                                                         
                                                                                      //_storageTransform = ResourceManager.GetStorage_Static(); // get any of the storage transforms!
                            _storageTransform = ResourceManager.GetStorageEntityNearPosition_Static(_thisUnit.GetPosition(), _resourceEntity.GetResourceType()); // get the storage transform!
                            _resourceEntity = ResourceManager.GetResourceEntityNearPosition_Static(_resourceEntity.GetPosition(), _resourceEntity.GetResourceType()); // if entity depledet look for another on near the depleadet one
                            _state = State.MoveToStorage;
                        } else {
                            if (_iCarry != _resourceEntity.GetResourceType()) { // if we already carrying a different resource, drop inventory
                                DropInventory();
                            }
                            _iCarry = _resourceEntity.GetResourceType(); // ref for what the unit carrys

                            // Gather Resource                                             
                            switch (_resourceEntity.GetResourceType()) {
                                case DataManager.ResourceType.Iron:
                                    _thisUnit.PlayAnimationMine(_resourceEntity.GetPosition(), GrabResourceFromEntity); // play animation, then, on animation complete Action calls the: GrabResourceFromEntity ()!
                                    break;
                                case DataManager.ResourceType.Alloy:
                                    _thisUnit.PlayAnimationMine(_resourceEntity.GetPosition(), GrabResourceFromEntity);
                                    break;
                                case DataManager.ResourceType.Food:
                                    _thisUnit.PlayAnimationMine(_resourceEntity.GetPosition(), GrabResourceFromEntity);
                                    break;
                                case DataManager.ResourceType.IronOre:
                                    _thisUnit.PlayAnimationMine(_resourceEntity.GetPosition(), GrabResourceFromEntity);
                                    break;
                                case DataManager.ResourceType.BauxiteOre:
                                    _thisUnit.PlayAnimationMine(_resourceEntity.GetPosition(), GrabResourceFromEntity);
                                    break;
                            }
                        }
                    }
                    break;

                case State.MoveToStorage:
                    Debug.Log("ONCE");
                    if (_storageTransform != null) { // if we have a storage move to it
                        if ((_thisUnit.IsIdle() ) && GetTotalInventoryAmount() > 0f) {
                            _thisUnit.SendAIToTarget(_storageTransform.position, () => {
                                DropInventoryIntoGameResources(); // empty inventory
                                UpdateUI();
                                _state = State.NotGathering;
                            });
                        } else {
                            _state = State.NotGathering;
                        }
                    } else if(_storageTransform == null && _state == State.MoveToStorage) {
                        _storageTransform = ResourceManager.GetStorageEntityNearPosition_Static(_thisUnit.GetPosition(), _resourceEntity.GetResourceType()); // cooroutine
                    }
                    break;
            }
        }

        #region AI
        public float GetTotalInventoryAmount() {
            float total = 0;

            foreach (DataManager.ResourceType resourceType in System.Enum.GetValues(typeof(DataManager.ResourceType))) {
                total += _inventoryAmountDicctonary[resourceType];
            }
            return total;
        }

        private void UpdateUI() { // DEL
            float inventoryAmount = GetTotalInventoryAmount();
            if (inventoryAmount > 0) {
                _inventoryTextMesh.text = "" + inventoryAmount;
            } else {
                _inventoryTextMesh.text = "";
            }
        }

        private void DropInventoryIntoGameResources() {
            foreach (DataManager.ResourceType resourceType in System.Enum.GetValues(typeof(DataManager.ResourceType))) {
                DataManager.Instance.AddResourceAmount(resourceType, _inventoryAmountDicctonary[resourceType]); // add to data
                _inventoryAmountDicctonary[resourceType] = 0f; // empty inventory
                _iCarry = resourceType;
            }
        }

        private void DropInventory() {
            foreach (DataManager.ResourceType resourceType in System.Enum.GetValues(typeof(DataManager.ResourceType))) {
                _inventoryAmountDicctonary[resourceType] = 0f; // empty inventory
                _iCarry = resourceType;
            }
        }

        public void ResetGatherer() {
            _resourceEntity = null;
            _state = State.NotGathering;
        }

        public void MovedToStorage(Transform storageTransform) {
            _storageTransform = storageTransform;
            _state = State.MoveToStorage;
            Debug.Log("Player Moved me to Storage");
        }

        public void SetResourceEntity(Resource resourceEntity) {
            this._resourceEntity = resourceEntity; //set target resourceEntity
        }

        private bool IsInventoryFull() {
            return GetTotalInventoryAmount() >= _thisUnit.MaxInventoryAmount;
        }

        private void GrabResourceFromEntity() {
            if (intakeCapacity > _thisUnit.MaxInventoryAmount) { // make sure it doesent take more than it can carry
                intakeCapacity = _thisUnit.MaxInventoryAmount;
            }

            float actualAmountTaken; // store amount we want to take
            DataManager.ResourceType resourceType = _resourceEntity.GrabResourceType(intakeCapacity, out actualAmountTaken); // tell how mutch this gatherer took away and get the resource type he took
            _inventoryAmountDicctonary[resourceType] += actualAmountTaken; // add it
            UpdateUI();
        }
        #endregion
    }
}

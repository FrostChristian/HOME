using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HOME.UI;
using HOME.Data;
using System;
using UnityEngine.AI;


namespace HOME.Game {

    [RequireComponent(typeof(Interactive))]
    [RequireComponent(typeof(Player))]
    public abstract class Entity : BaseInteraction {
        //public enum EntityTypes { none, unit, building, resource };

        //events
        public static event EventHandler OnStorageEntityClicked;

        [Header("General Info")]
        [SerializeField] private string _name = "Entitys_name"; //the name of the entity that will be displayd when it is selected.
        public string GetName { get { return _name; } set { _name = value; } }

        [SerializeField] private string _description = "Entitys_description"; //the description of the entity that will be displayed when it is selected.
        public string GetDescription { get { return _description; } set { _description = value; } }

        [SerializeField] private Sprite _icon = null; //the icon that will be displayed when the faction entity is selected.
        public Sprite GetIcon { get { return _icon; } set { _icon = value; } }

        [Header("Health")]
        public bool hasHealth = true;
        [SerializeField] private float maxHealth = 100f;
        public float MaxHealth { get { return maxHealth; } set { if (value > 0.0) maxHealth = value; } }    //the maximum health must always be > 0.0

        [SerializeField] private float _currHealth;
        public float CurrHealth { get { return _currHealth; } set { _currHealth = value; } }    //the maximum health must always be > 0.0

        [SerializeField] private float deathHealth = 0f;
        public GameObject explosionPrefab;

        [Header("Resources")]
        public bool hasResources = true;
        [SerializeField] public float _currInventoryAmount = 1f;
        [SerializeField] public float _maxInventoryAmount = 5000f;

        [Space]
        public bool isStorage = false;
        [SerializeField] public DataManager.ResourceType resourceStorageType; // assign in inspector

        [Header("Cost")]
        [SerializeField] private float _ironCost = 0f;
        public float GetIronCost { get { return _ironCost; } }
        [SerializeField] private float _alloyCost = 0f;
        public float GetAlloyCost { get { return _alloyCost; } }
        [SerializeField] private float _foodCost = 0f;
        public float GetFoodCost { get { return _foodCost; } }
        [SerializeField] private float _energyCost = 0f;
        public float GetEnergyCost { get { return _energyCost; } }


        [Header("Unity Info")]
        [HideInInspector] public InGameMenu _inGameMenu;
        [HideInInspector] public PlayerSetupDefinition player;
        public bool showUI = false;
        public bool Selected { get { return _selected; } set { _selected = value; } }
        [SerializeField] private bool _selected = false;
        public GameObject spawnPoint;
        public Action ClickFunc = null;
        private ShowActionButton _buttons;
        private Interactive _interactive;
        [SerializeField] private QuestManager _questManager;

        void OnMouseOver() {
            if (Input.GetMouseButtonUp(1)) { // only on rightclick
                if (MouseManager.IsPointerOverUI()) {
                    return; // Over UI!
                }
                ClickFunc?.Invoke();
            }
        }

        public virtual void Awake() {
            CheckPositionOnNavMesh();
            _interactive = GetComponent<Interactive>();
            if (!player.isAi && !tag.Contains("Ore")) {
                if (isStorage) {
                    ClickFunc = () => { //if i clicked this storage fire event
                        OnStorageEntityClicked?.Invoke(spawnPoint, EventArgs.Empty);
                    }; // send this to subscriber
                }
                _buttons = GetComponent<ShowActionButton>();

                CheckQuest();
                //_questManager = GetComponent<QuestManager>();
                _questManager = FindObjectOfType(typeof(QuestManager)) as QuestManager;

                if (isStorage) {
                    ResourceManager.Instance.ironStorageTransform.Add(spawnPoint.transform);
                }
            }


        }

        private void CheckQuest() {
            QuestSetupDefinition currentQuest = QuestManager.Instance.GetCurrentQuest();
            if (tag == "DefenseEntity" && currentQuest.Id == 2) {
                QuestManager.Instance.GetCurrentQuest().Progress++;
                //Debug.Log(" QuestManager.Instance.GetCurrentQuest().Progress" + QuestManager.Instance.GetCurrentQuest().Progress);
            } else {
            }
        }



        public virtual void Start() {
            if (GetComponent<Resource>() == null) {
                player = GetComponent<Player>().Info;
            }
            _inGameMenu = FindObjectOfType<InGameMenu>();

            if (tag == "Ship") { // is this the player Ship?
                _currHealth = DataManager.Instance.PlayerShipHealth;
                MaxHealth = DataManager.Instance.PlayerShipMaxHealth;
            } else {
                _currHealth = MaxHealth;
            }
        }

        public virtual void Update() {
            CheckHealth();
            if (!showUI) {
                return;
            }
            InGameMenuUpdate();
        }

        public override void Select() {
            showUI = true;
            Selected = true;
        }

        public override void Deselect() {
            if (_inGameMenu != null) {
                _inGameMenu.SelectionClear();
            }
            Selected = false;
            showUI = false;
        }

        public bool CheckPositionOnNavMesh() {
            NavMeshHit hit; // make sure spawn is on the navmesh
            if (NavMesh.SamplePosition(transform.position, out hit, 20, NavMesh.AllAreas)) {
                transform.position = hit.position;
                return true;
            } else {
                return false;
            }
        }

        private void CheckHealth() {
            if (CurrHealth <= deathHealth) {
                if (_interactive.Selected) { // if selected deselect
                    Deselect(); //Clear Infopanel
                }
                if (_buttons != null) { // If Unit had Buttons
                    _buttons.Deselect(); //Clear Buttons
                }
                Instantiate(explosionPrefab, transform.position, Quaternion.identity); //DestroyEffect
                MouseManager.Instance.selectedUnits.Remove(_interactive); // remove from selected list
                Destroy(this.gameObject);
            }
        }
        //------------------------------------------ CostCheck--------------------------------------------//
        public bool CheckCost(bool addTooltip) {
            if (GetIronCost > DataManager.Instance.GetResourceAmount(DataManager.ResourceType.IronOre)) {
                if (addTooltip) TooltipWarning.ShowTooltip_Static("Not enough Iron");
                return false;
            }
            if (GetAlloyCost > DataManager.Instance.GetResourceAmount(DataManager.ResourceType.BauxiteOre)) {
                if (addTooltip) TooltipWarning.ShowTooltip_Static("Not enough Alloy");
                return false;
            }
            if (GetFoodCost > DataManager.Instance.GetResourceAmount(DataManager.ResourceType.Food)) {
                if (addTooltip) TooltipWarning.ShowTooltip_Static("Not enough Food");
                return false;
            }
           
            return true;
        }
        public void GetCost() {
            DataManager.Instance.AddResourceAmount(DataManager.ResourceType.IronOre, -GetIronCost);
            DataManager.Instance.AddResourceAmount(DataManager.ResourceType.BauxiteOre, -GetAlloyCost);
            DataManager.Instance.AddResourceAmount(DataManager.ResourceType.Food, -GetFoodCost);
        }

        //------------------------------------------ +CostCheck--------------------------------------------//
        public void ModifyEntityHealth(Entity target, float value) { // modyfy any entity
            target.CurrHealth += value;
            if (target.CurrHealth <= 0.0f) {// if unit gets below 0 health
                                            //destroy obj //destroy unit
            }
            if (target.CurrHealth >= MaxHealth) { // if unit gets healed over max health
                target.CurrHealth = MaxHealth; // set curr health to max health
            }
        }

        public void DestroyEntity() {
            CurrHealth = 0f;
        }

        public virtual void InGameMenuUpdate() {
            //Debug.Log("Shout");
            if (_inGameMenu == null) {
                _inGameMenu = FindObjectOfType<InGameMenu>(); //get Ingamemenu
                Debug.Log("Entity; INGAMEMENUUPDATE Had to assign IngameMenu");
            }
        }
    }
}




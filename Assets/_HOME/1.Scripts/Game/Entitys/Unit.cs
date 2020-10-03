using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HOME.Game {

    public class Unit : Entity {

        public enum State { Idle, Building, Gathering, Moving, Animating }

        [Header("Unit Info")]
        [SerializeField] private float _speed = 20f;
        public float Speed { get => _speed; set => _speed = value; }
        public float stopDistance = 10f;
        public static bool _hasMinimapTarget = false;
        public static Vector3 _minimapTarget = Vector3.zero;

        [Space]
        [SerializeField] private State state = default;
        [SerializeField] private Vector3 _target = default;

        private GameManager _gameManager;
        private PlayerSetupDefinition _player;
        [HideInInspector] public GatherAI _gathererAI;

        private NavMeshAgent _agent;
        public NavMeshAgent Agent { get => _agent; }

        private NavMeshObstacle _obstacle;
        public List<GameObject> eCrates = new List<GameObject>();


        private Action _onArrivedAtPosition;

        private Vector3 lastPosition;
        private float moveTimer;

        public override void Start() {
            state = State.Idle;
            base.Start();

            //get references
            _agent = GetComponent<NavMeshAgent>();
            _obstacle = GetComponent<NavMeshObstacle>();
            _player = GetComponent<Player>().Info;
            _gathererAI = GetComponent<GatherAI>();
            _gameManager = FindObjectOfType<GameManager>();

            if (_gathererAI != null) {
                _gathererAI.Init(this);
            }

            _agent.enabled = false;
            _obstacle.carving = true; //enable carving
            _obstacle.carveOnlyStationary = true; //carve only stationary 
            _obstacle.enabled = true;
            RefreshTimer();
        }

        public void FixedUpdate() {
            if (eCrates.Count > 0) {
                HandleCrates(); // change look of entity
            }
            if (Selected) {
                InGameMenuUpdate();
            }
            switch (state) {
                case State.Idle:
                    WaitingForTarget();
                    break;

                case State.Moving:
                    WaitingForTarget();
                    break;
                case State.Animating:
                    break;
            }
        }
        // -----------------------------------------------AI Behavior -----------------------------------------------//
        public bool IsIdle() { // GathererAI requires this
            return state == State.Idle;
        }

        public void PlayAnimationMine(Vector3 lookAtPosition, Action onAnimationCompleted) { // use for animations
            state = State.Animating;

            state = State.Idle;
            onAnimationCompleted?.Invoke();
        }

        public void SendAIToTarget(Vector3 pos, Action onArrivedAtPosition) { // for AI
            _onArrivedAtPosition = onArrivedAtPosition;
            _agent = GetComponent<NavMeshAgent>();
            _target = pos;
            state = State.Moving;
            StartCoroutine(NavMeshAgentObsticalSwitch());
        }
        // ----------------------------------------------- +AI Behavior -----------------------------------------------//
        // ----------------------------------------------- Movement -----------------------------------------------//

        public void WaitingForTarget() {
            if (_hasMinimapTarget) {
                SendToMiniMapTarget(_minimapTarget);
                _hasMinimapTarget = false;
                return;
            }

            if (Selected // are we selected?
                && Input.GetMouseButtonUp(1) // was right mouse clicked? 
                && MouseManager.Instance.enabled  // MM enabeled?
                && !player.isAi // is not AI?
                && !MouseManager.IsPointerOverUI()) { // is Pointer over UI?

                // all yes? then go!                                       

                Vector3? tempTarget = _gameManager.ScreenPointToMapPosition(Input.mousePosition);
                if (tempTarget.HasValue) {
                    _target = tempTarget.Value; // set temp value to actve target

                    if (_gathererAI != null) { // let the gatherer know the player klicked!
                        _gathererAI.ResetGatherer();
                    }
                    MouseManager.Instance.MarkTarget(_target, Color.red);
                    _onArrivedAtPosition = null;  // cancel action
                    Debug.Log("Cancel Action");
                    StartCoroutine(NavMeshAgentObsticalSwitch());
                } else {
                    Debug.Log("NO TARGET");
                }
            }
        }

        public void SendToMiniMapTarget(Vector3 miniMapTarget) {
            if (Selected // are we selected?
                && MouseManager.Instance.enabled  // MM enabeled?
                && !player.isAi) { // is not AI?

                // all yes? then go!
                if (_gathererAI != null) { // let the gatherer know the player klicked!
                    _gathererAI.ResetGatherer();
                }

                _target = miniMapTarget;

                MouseManager.Instance.MarkTarget(_target, Color.yellow);
                _onArrivedAtPosition = null;  // cancel action
                StartCoroutine(NavMeshAgentObsticalSwitch());
            }
        }

        IEnumerator NavMeshAgentObsticalSwitch() {
            if (_obstacle.enabled == true) { // if we already moving skip this
                _obstacle.carving = false; //enable carving
                _obstacle.enabled = false;
                yield return 1;
            }

            if (CheckPositionOnNavMesh()) { // only do this when we are on the navmesh
                _agent.enabled = true;
                _agent.isStopped = false;
                _agent.SetDestination(_target);
                state = State.Moving;
                StartCoroutine(CheckAgentDestinations());
            } else {
                Debug.Log("Unit Checkposonnavmesh ERROR");
            }
        }

        IEnumerator CheckAgentDestinations() {
            while (Vector3.Distance(transform.position, _target) > stopDistance) {

                if (moveTimer > 0) { //movement check timer -> making sure the unit is not stuck at its current position
                    moveTimer -= Time.deltaTime;
                }
                if (moveTimer < 0.1) {//the movement check duration is hardcoded to 2 seconds, while this is only a temporary solution for the units getting stuck issue, a more optimal solution will be soon presented

                    if (Vector3.Distance(transform.position, lastPosition) <= 0.2f) { // if unit stuck give  itmore accses!
                        CheckPositionOnNavMesh();
                        if (_gathererAI != null) {
                            Debug.Log("Widen NavemashAgents Options");
                            stopDistance += .5f;
                            _agent.avoidancePriority += 1;
                        }
                    }
                    RefreshTimer();
                }
                yield return new WaitForSeconds(.2f);
            }

            if (_agent.hasPath) {
                _agent.ResetPath();
                _agent.isStopped = true;
                _agent.enabled = false;

                _obstacle.enabled = true;
                _obstacle.carving = true; //enable carving
            }

            if (_onArrivedAtPosition != null) {
                Action tmpAction = _onArrivedAtPosition;
                _onArrivedAtPosition = null;
                tmpAction();
            }
            state = State.Idle;
        }
        // ----------------------------------------------- +Movement -----------------------------------------------//

        private void HandleCrates() { // for optical representation of resurces
            GetInventoryAmount();
            if (eCrates.Count > 0) { // if crates available
                float amoutPerCrate = MaxInventoryAmount / eCrates.Count;
                for (int i = 0; i < eCrates.Count; i++) {
                    if (_currInventoryAmount > amoutPerCrate * i) {
                        eCrates[i].SetActive(true);
                    } else {
                        eCrates[i].SetActive(false);
                    }
                }
            }
        }
        private void RefreshTimer() {
            moveTimer = 0.20f; //launch the timer
            lastPosition = transform.position; //set this is as the last registered position.
        }

        #region InGameMenuUpdate 

        public override void InGameMenuUpdate() {
            base.InGameMenuUpdate();
            GetInventoryAmount();
            float healthBar = CurrHealth / MaxHealth;
            _inGameMenu.SelectionFill(
                GetName,
                hasHealth,
                CurrHealth + "/" + MaxHealth,
                GetDescription,
                GetIcon,
                player.isAi,
                healthBar,
                hasResources,
                GetInventoryDescription(),
                _currInventoryAmount / MaxInventoryAmount,
                GetInventoryAmount());
        }

        public string GetInventoryDescription() {
            if (_gathererAI != null && GetInventoryAmount() != "0") {
                return "Inventory: " + _gathererAI._iCarry;
            }
            if (_gathererAI != null && GetInventoryAmount() == "0") {
                return "Inventory: Empty";
            }
            return "";
        }

        private float GetInventorySliderAmount() {
            return _currInventoryAmount / MaxInventoryAmount;
        }

        public string GetInventoryAmount() {
            if (_gathererAI != null) {
                var invAmount = _gathererAI.GetTotalInventoryAmount();
                _currInventoryAmount = invAmount;
                return invAmount.ToString();
            }
            return "";
        }

        public void UpdateInverntoryText(string resourceType, float totalInventoryAmount) {
            InGameMenuUpdate();
        }
        #endregion
    }
}
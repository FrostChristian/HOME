using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using HOME.Data;
using HOME.UI;

namespace HOME.Game {

    public class GameManager : MonoBehaviour {
        private static GameManager _instance;
        public static GameManager Instance { get => _instance; }
        //-------------------------------------------TEMP--------------------------------------------//   
        [SerializeField] private bool _debugWin;
        public bool DebugWin { get { return _debugWin; } set { _debugWin = value; } }
        //-------------------------------------------+TEMP--------------------------------------------//

        private bool _isGameOver;
        public bool IsGameOver { get { return _isGameOver; } set { _isGameOver = value; } }
        private bool GAMEOVER = false;
        public bool iWon = false;

        [SerializeField] private TransitionFader _winTransitionPrefaN;
        [SerializeField] private TransitionFader _loseTransitionPrefaN;
        //-------------------------------------------PLayerSetupDefinitions--------------------------------------------//
        [Header("Players")]
        [SerializeField] private Transform _playerParent;
        [SerializeField] private GameObject _defaultUnitPrefab;
        [SerializeField] private GameObject _defaultBasePrefab;
        [SerializeField] private GameObject _defaultAIBasePrefab;
        [SerializeField] private List<Transform> _humanSpawn = new List<Transform>();
        [SerializeField] private List<Transform> _aISpawn = new List<Transform>();
        public List<PlayerSetupDefinition> Factions = new List<PlayerSetupDefinition>();
        //------------------------------------------+PLayerSetupDefinitions--------------------------------------------//
        //------------------------------------------ Navigation--------------------------------------------//
        [SerializeField] public Camera _miniMapCamera;
        [SerializeField] public static TerrainCollider _mapCollider;
        //------------------------------------------+Navigation--------------------------------------------//
        //------------------------------------------ Repair--------------------------------------------//
        public static bool _isRepairActive = false;
        //------------------------------------------ +Repair--------------------------------------------//
        //------------------------------------------ AI--------------------------------------------//
        [SerializeField] private bool _aIEnabled = true;
        [SerializeField] bool _spawnEnemys = true;
        [SerializeField] private GameObject[] _aIEntitys;
        [SerializeField] private NavMeshObstacle[] navMeshObstacles;

        //------------------------------------------- +AI--------------------------------------------//
        //------------------------------------------ Highlights Range Indicator--------------------------------------------//
        public static List<Projector> playerRangeProjectors = new List<Projector>();
        [SerializeField] private bool _rangeProjectorsEnabled;
        //------------------------------------------ +Highlights Range Indicator--------------------------------------------//
        //------------------------------------------ Loading --------------------------------------------//
        private GameDifficultyDefinition _currentDifficulty;
        private GameDifficultyDefinition _currentQuest;

        //------------------------------------------ + Loading--------------------------------------------//


        //scripts
        public MouseManager MouseMgr { private set; get; }
        public ResourceManager ResourceMgr { private set; get; }
        public CameraManager CamMgr { private set; get; }
        public DifficultyManager DifficultyMgr { private set; get; }
        public QuestManager QuestMgr { private set; get; }
        public InGameMenu _inGameMenu { private set; get; }

        private void Awake() {
            if (_instance != null) {//singleton
                Destroy(gameObject);
            } else {
                _instance = this; // global
            }

            //get managers
            MouseMgr = FindObjectOfType(typeof(MouseManager)) as MouseManager;
            ResourceMgr = FindObjectOfType(typeof(ResourceManager)) as ResourceManager;
            CamMgr = FindObjectOfType(typeof(CameraManager)) as CameraManager;
            DifficultyMgr = FindObjectOfType(typeof(DifficultyManager)) as DifficultyManager;
            QuestMgr = FindObjectOfType(typeof(QuestManager)) as QuestManager;
           

            // init managers
            ResourceMgr.Init(this);
            MouseMgr.Init(this);
            CamMgr.Init(this);


            // player Data
            DifficultyMgr.SetIndex(DataManager.Instance.AiDifficultyIndex);
            _currentDifficulty = DifficultyMgr?.GetCurrentDifficulty();

            if (SceneManager.GetActiveScene().name == "Map1") {
                InGameMenu.Open();
                Debug.Log("GameManager Awake(): Entered Game through Map1");
            }

            DataManager.Instance.AddResourceAmount(DataManager.ResourceType.IronOre, 500);
            DataManager.Instance.AddResourceAmount(DataManager.ResourceType.BauxiteOre, 500);
            DataManager.Instance.AddResourceAmount(DataManager.ResourceType.Food, 100);
            //DataManager.Instance.AddResourceAmount(DataManager.ResourceType.Food, 5000);
            //DataManager.Instance.AddResourceAmount(DataManager.ResourceType.Alloy, 5000);
            PrepareLevel();
            _rangeProjectorsEnabled = true;
            InGameMenu._initQuests =true;
        }

        private void Update() {
            CheckEndGame();
            RangeIndicatorDisplay();
            RepairShip();
        }

        #region PrepareLevel

        public void PrepareLevel() {
            //Debug.Log("GameManager: Preparing level");
            FindMap();
            FindSpawnPoints();
            CreatePlayers();
            PopulatePlayers();
        }

        private void FindMap() {
            _mapCollider = FindObjectOfType<TerrainCollider>();
            _mapCollider.gameObject.tag = "Terrain";
            MiniMap.terrainSize = _mapCollider.terrainData.size;
            if (_mapCollider == null) {
                Debug.Log("GAMEMANAGER FindMap no collider found");
            }
        }

        public void FindSpawnPoints() {
            //Debug.Log("GameManager: Find Spawnpoints");
            var hSpawn = GameObject.FindGameObjectsWithTag("HumanSpawn");
            var aSpawn = GameObject.FindGameObjectsWithTag("AISpawn");
            for (int i = 0; i < hSpawn.Length; i++) {
                var hs = hSpawn[i].transform;
                _humanSpawn.Add(hs);
            }
            for (int i = 0; i < aSpawn.Length; i++) {
                var ai = aSpawn[i].transform;
                _aISpawn.Add(ai);
            }
        }

        private void CreatePlayers() {
            foreach (var hS in _humanSpawn) {//create human player
                var newPlayer = new PlayerSetupDefinition();
                newPlayer.playerID = 0; //set ID
                newPlayer.playerName = DataManager.Instance.PlayerName; //set loaction to transform
                newPlayer.playerLocation = hS; //set loaction to transform
                newPlayer.isAi = false; //set ai false
                newPlayer.playerCredits = 300f; // DEL
                //newPlayer.StartingEntitys.Add(_defaultUnitPrefab);
                newPlayer.StartingEntitys.Add(_defaultBasePrefab);
                Factions.Add(newPlayer);
            }

            foreach (var aS in _aISpawn) { //create Ai player
                PlayerSetupDefinition newAI = new PlayerSetupDefinition();
                newAI.playerID = 1;
                newAI.playerName = DataManager.Instance.AIName;
                newAI.playerLocation = aS;
                newAI.isAi = true;
                newAI.playerCredits = 300f;
                newAI.StartingEntitys.Add(_defaultAIBasePrefab);
                //newAI.StartingEntitys.Add(_defaultUnitPrefab);
                Factions.Add(newAI);
            }
        }

        private void PopulatePlayers() {
            //Debug.Log("GameManager: Populate Players");
            int iter = 0;
            foreach (var faction in Factions) {
                iter++; // just for parent naming
                faction.playerName = faction.playerName + " " + iter.ToString(); // set Player Name
                faction.playerLocation.name = "Player: " + faction.playerName; // rename parent

                foreach (var entity in faction.StartingEntitys) { //all startingunits in each player
                    if (!_spawnEnemys && faction.isAi) {
                        //Debug.Log("GameManager: Populate Players() Spawn Enemys Disabled");
                        return;
                    }

                    Vector3 spawnPoint = faction.playerLocation.position;
                    if (entity.GetComponent<Unit>() != null) { // if it is a movable unit then offset it by xx so its not spawned directly on the base
                        spawnPoint = new Vector3(faction.playerLocation.position.x + 10f, faction.playerLocation.position.y, faction.playerLocation.position.z - 10f);
                        NavMeshHit hit; // make sure spawn is on the navmesh
                        if (NavMesh.SamplePosition(spawnPoint, out hit, 20, NavMesh.AllAreas)) {
                            spawnPoint = hit.position;
                        }
                    }
                    GameObject newEntity = Instantiate(entity, spawnPoint, faction.playerLocation.rotation, faction.playerLocation.transform); //, menuParentObject.transform); // instanciate unit at player startTransform         

                    if (!newEntity.GetComponent<Player>()) {
                        Debug.Log("No Player Script on: " + entity);
                        continue;
                    }

                    Player player = newEntity.GetComponent<Player>(); // add player component to newUnit
                    player.Info = faction;

                    if (!faction.isAi) {
                        if (Player.humanPlayer == null) {
                            Player.humanPlayer = faction; // .Info AND .humanPlayer == this faction!
                        }
                        newEntity.GetComponent<Player>().Info.isAi = false; // Mark the Unit as non-AI
                    } else {
                        if (newEntity.GetComponent<ShowActionButton>() != false) { // if Ai hide buttons
                            Destroy(newEntity.GetComponent<ShowActionButton>());
                        }

                    }
                }

                if (_aIEnabled && faction.isAi) {
                    CreateAI(faction.playerName, faction.playerLocation.transform);
                }
            }
        }
        #endregion

        //------------------------------------------ AI--------------------------------------------//
        private void CreateAI(string aIName, Transform parent) {
            if (_currentDifficulty == null) {
                Debug.Log("GameManager: Create AI No difficulty Found");
                return;
            }
            if (_currentDifficulty != null) {
                Debug.Log("GameManager: Create AI +" + aIName + "Difficulty ID = " + _currentDifficulty.Id);
                for (int i = 0; i < _aIEntitys.Length; i++) {
                    //if difficulty matches Ai use it 
                    if (_currentDifficulty.Id == i) {
                        GameObject aIEntity = Instantiate(_aIEntitys[i], parent);
                        aIEntity.GetComponent<AIController>().playerName = aIName;
                    }
                }
            } else {

                Debug.Log("GAMEMANAGER CreateAI() NO CURRENT DIFFICULTY COULD BE FOUND");
            }
        }
        //------------------------------------------- +AI--------------------------------------------//
        //------------------------------------------ Navigation--------------------------------------------//
        public Vector3? ScreenPointToMapPosition(Vector2 point) { // returns mouse pos pos on terrain hight
            Ray ray = Camera.main.ScreenPointToRay(point);
            RaycastHit hit;
            if (!_mapCollider.Raycast(ray, out hit, Mathf.Infinity)) {
                return null;
            }
            return hit.point;
        }
        //------------------------------------------+Navigation--------------------------------------------//

        //------------------------------------------Building Placement--------------------------------------------//
        public bool IsEntitySafeToPlace(GameObject entity) {
            var obstacles = FindObjectsOfType<NavMeshObstacle>();
            var cols = new List<Collider>();

            foreach (var o in obstacles) {
                if (o.gameObject != entity) {
                    cols.Add(o.gameObject.GetComponent<Collider>());
                }
            }
            var verts = entity.GetComponentInChildren<MeshFilter>().mesh.vertices;
            foreach (var v in verts) {
                NavMeshHit hit;
                var vReal = entity.transform.TransformPoint(v);
                NavMesh.SamplePosition(vReal, out hit, 20, NavMesh.AllAreas);

                bool onXAxis = Mathf.Abs(hit.position.x - vReal.x) < 0.5f;
                bool onZAxis = Mathf.Abs(hit.position.z - vReal.z) < 0.5f;
                bool hitCollider = cols.Any(c => c.bounds.Contains(vReal));

                if (hitCollider) {
                    return false;
                }
            }
            return true;
        }
        // */
        /*
                     navMeshObstacles = null;
                     List<MeshFilter> mfilters = new List<MeshFilter>(entity.GetComponentsInChildren<MeshFilter>()); // collect all meshes on entity
                     List<Vector3[]> meshVertList = new List<Vector3[]>(); // list with verts for each mesh on entity
                     List<Collider> colliderList = new List<Collider>(); // list for all collidable objets
                     navMeshObstacles = FindObjectsOfType<NavMeshObstacle>(); // find all obsticles

                     foreach (MeshFilter filter in mfilters) { // get verts for each mesh
                         if (filter.mesh.isReadable) { // only add readable meshes
                             meshVertList.Add(filter.mesh.vertices);
                         }
                     }

                     foreach (var obsticle in navMeshObstacles) { // get colliders of obsticles
                         if (obsticle.gameObject != entity) { // ignore own obsticle
                             colliderList.Add(obsticle.gameObject.GetComponent<Collider>()); // add colliders
                         }
                     }

                     foreach (var meshVert in meshVertList) {
                         foreach (var vert in meshVert) {
                             NavMeshHit hit;
                             Vector3 vertWorldPos = entity.transform.TransformPoint(vert); // get vert world position
                             NavMesh.SamplePosition(vertWorldPos, out hit, 50, NavMesh.AllAreas); // vert pos to NavMesh
                 //Debug.Break();
                             bool onXAxis = Mathf.Abs(hit.position.x - vertWorldPos.x) < 0.5f; // is distance x in range?
                             bool onZAxis = Mathf.Abs(hit.position.z - vertWorldPos.z) < 0.5f; // is distance y in range? 
                             foreach (Collider collider in colliderList) {
                                 //Debug.Log("Collider: "+ collider.bounds.Contains(vertWorldPos));
                             }
                             bool hitCollider = colliderList.Any(collider => collider.bounds.Contains(vertWorldPos)); // is this vert inside of a collider
                 //Debug.Log("1: x "+ onXAxis);
                 //Debug.Log("1: y "+ onZAxis);
                 Debug.Log("1: Collider "+ hitCollider);
                             if (!onXAxis || !onZAxis || hitCollider) { // not save to place!
                                 return false;
                             }
                         }
                     }
                     return true;                        
     }*/

        //-----------------------------------------+Buildin Placement--------------------------------------------//
        //------------------------------------------ Highlights Range Indicator--------------------------------------------//
        private void RangeIndicatorDisplay() {
            if (Input.GetKeyDown(KeyCode.Space) && playerRangeProjectors.Count > 0) {
                if (_rangeProjectorsEnabled) {
                    foreach (Projector projector in playerRangeProjectors) { // go through projectors
                        projector.enabled = false;
                        _rangeProjectorsEnabled = false; // switch
                    }
                    return;
                } else {
                    foreach (Projector projector in playerRangeProjectors) { // go through projectors
                        projector.enabled = true; // enable projector
                        _rangeProjectorsEnabled = true; // switch
                    }
                    return;
                }
            }
        }
        //------------------------------------------ +Highlights Range Indicator--------------------------------------------//
        //-----------------------------------------Debug stuff--------------------------------------------//
        // clear the particles out of the hierachy
        public static Transform InstanceGraveParent() { // returns parent for instances
            Transform _particleParent;
            if (GameObject.Find("ParticleContainer").transform == null) {
                GameObject _particleParentObject = new GameObject("ParticleContainer");
                _particleParent = _particleParentObject.transform;
            } else {
                _particleParent = GameObject.Find("ParticleContainer").transform;
            }
            if (_particleParent == null) {
                Debug.Log("GameManager: InstanceGraveParent() No parent found or made!");

            }
            return _particleParent;
        }
        //----------------------------------------- +Debug stuff--------------------------------------------//
        //-----------------------------------------Misc--------------------------------------------//
        private void RepairShip() {
            if (_isRepairActive) {

                float repairThreshholdAmount = 50f;

                if (DataManager.Instance.PlayerShipHealth >= DataManager.Instance.PlayerShipMaxHealth) {
                    DataManager.Instance.PlayerShipHealth = DataManager.Instance.PlayerShipMaxHealth;
                    return;
                }  

                DataManager.ResourceType repairMaterialType = DataManager.ResourceType.IronOre; // reference to repair Material type

                float repairMaterialAmount = DataManager.Instance.GetResourceAmount(repairMaterialType); // reference to Type amount

                if (repairMaterialAmount >= repairThreshholdAmount) {// do we have materials?
                    DataManager.Instance.AddResourceAmount(repairMaterialType, -repairThreshholdAmount);//take materials
                    Building.PlayerShip.CurrHealth += 1; // add health
                    Debug.Log(" pHealth " + DataManager.Instance.PlayerShipHealth);
                }                           
            } 
        }

        private void CheckEndGame() {

            if (_isGameOver && !GAMEOVER) {
                GAMEOVER = true;
                EndLevel(iWon);
                return;
            }

            if (!_isGameOver) {// check if we have set IsGameOver to true, only run this logic once
                foreach (var u in Factions) {
                    if (!u.isAi && u.ActiveUnits.Count <= 0) {
                        Debug.Log("GameManager CheckEndGame No active Player Units! Game Over!");
                        _isGameOver = true;
                        iWon = false;
                        return;
                    }
                    if (u.isAi && u.ActiveUnits.Count <= 0 && _spawnEnemys) {
                        _isGameOver = true;
                        iWon = true;
                        return;
                    }
                }
                if (_debugWin) {
                    _isGameOver = true;
                    iWon = true;
                    return;
                }
                if (DataManager.Instance.PlayerShipHealth <= 0) {
                    _isGameOver = true;
                    iWon = false;
                    return;
                }
            }
        }

        public void EndLevel(bool won) {// end the level
            Destroy(_playerParent);
            var go = GameObject.Find("AIManager");
            Destroy(go);
            if (won) {
                StartCoroutine(WinRoutine());
            } else if (!won) {
                StartCoroutine(LoseRoutine());
            }
        }

        private IEnumerator LoseRoutine() {
            TransitionFader.PlayTransition(_loseTransitionPrefaN);
            float fadeDelay = (_loseTransitionPrefaN != null) ? _loseTransitionPrefaN.Delay + _loseTransitionPrefaN.FadeOnDuration : 0f;
            yield return new WaitForSeconds(fadeDelay);
            LoseMenu.Open();
            Debug.Log("GameManager LoseRoutine(): LoseMenu.Open()");
        }

        private IEnumerator WinRoutine() {
            TransitionFader.PlayTransition(_winTransitionPrefaN);
            float fadeDelay = (_winTransitionPrefaN != null) ? _winTransitionPrefaN.Delay + _winTransitionPrefaN.FadeOnDuration : 0f;
            yield return new WaitForSeconds(fadeDelay);
            WinMenu.Open();
            Debug.Log("GameManager WinRoutine(): WinMenu.Open()");
        }

        public void ResetGame() { //dont need it?
            MouseManager.Instance.DeselectAll();
            _isGameOver = false;
            _debugWin = false;
            // clear Spawnpoints if we come from previous game
            _humanSpawn.Clear();
            _aISpawn.Clear();
            Debug.Log("RESET");
        }

        private void OnDestroy() {
            if (_inGameMenu != null) {

            StopCoroutine(_inGameMenu.CheckQuestCoroutine());
            }
        }

        public void QuitApplication() {
            Debug.LogWarning("GAMEMANAGER QUITAPPLICATION Quit Application!");
        }
    }
}
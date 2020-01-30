using UnityEngine;

namespace HOME.Game {

    public class FindBuildingSite : MonoBehaviour {
        public float maxBuildDistance = 30f; // away from entity
        public GameObject BuildingPrefab;
        public PlayerSetupDefinition Info;
        public Entity thisEntity;
        public Transform Source;

        Renderer[] rend;
        Color Red = new Color(1, 0, 0, .5f);
        Color Green = new Color(0, 1, 0, .5f);
        private GameManager _gameManager;

        private void Start() {
            _gameManager = FindObjectOfType<GameManager>();
            MouseManager.Instance.enabled = false;// DEL 09_05
            rend = GetComponentsInChildren<Renderer>();
        }

        void Update() {
            var tempTarget = _gameManager.ScreenPointToMapPosition(Input.mousePosition);
            if (tempTarget.HasValue == false)
                return;
            transform.position = tempTarget.Value;

            if (Vector3.Distance(transform.position, Source.position) > maxBuildDistance) {
                foreach (var mat in rend) {
                    mat.material.color = Red;
                }
                return;
            }

            if (_gameManager.IsEntitySafeToPlace(gameObject)) {
                foreach (var mat in rend) {
                    mat.material.color = Green;
                }
                if (Input.GetMouseButtonUp(0)) {
                    BuildingPrefab.GetComponent<Entity>().GetCost();
                    GameObject building = Instantiate(BuildingPrefab, Info.playerLocation.transform);
                    building.AddComponent<ShowActionButton>(); //get action buttons
                    building.transform.position = transform.position;
                    if (building.GetComponent<Player>() != null) { // if player script already there
                        building.GetComponent<Player>().Info = Info; // set it
                    } else { // if not
                        building.AddComponent<Player>().Info = Info; // add it ant set it
                    }
                    Destroy(gameObject);
                }
            } else {
                foreach (var mat in rend) {
                    mat.material.color = Red;
                }
            }
        }
        private void OnDestroy() {
            MouseManager.Instance.enabled = true;
        }
    }
}
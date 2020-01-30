using UnityEngine;

namespace HOME.Game {

    public class CreateBuildingAction : ActionBehavior {

        public GameObject buildingPrefab;
        public GameObject ghostBuildingPrefab;
        public float maxBuildDistance = 30;
        private GameObject _active = null;

        private void Start() {
            if (buildingPrefab.TryGetComponent(out Entity entity)) {// if we have a Entity on this
                _prefabEntity = entity;
            } else {
                Debug.Log("CreateBuildingAction! Start()  NO ENTITY ON PREFAB!");
            }
        }

        public override System.Action GetClickAction() { // add clickaction to the button
            return delegate () {

                if (_prefabEntity.CheckCost(true)) {// check for resources
                    if (_active == null) {
                        GameObject ghostBuilding = Instantiate(ghostBuildingPrefab);
                        if (ghostBuilding.GetComponent<AttackInRange>() != null) { // if we have an attack script
                            ghostBuilding.GetComponent<AttackInRange>().enabled = false; // disable it
                        }
                        FindBuildingSite finder = ghostBuilding.AddComponent<FindBuildingSite>();
                        finder.BuildingPrefab = buildingPrefab;
                        finder.maxBuildDistance = maxBuildDistance;
                        finder.Info = GetComponent<Player>().Info;
                        finder.Source = transform;
                        _active = ghostBuilding;
                    }
                } else {
                    Debug.Log("Not enough resources!");
                }
            };
        }

        void Update() {
            if (_active == null) {
                return;
            }
            if (Input.GetMouseButtonDown(0) && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) { // if click on ui element end building placement
                Destroy(_active);
                return;
            }
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) {
                Destroy(_active);
                return;
            }
        }

        private void OnDestroy() {
            if (_active == null)
                return;
            Destroy(_active);
        }
    }
}

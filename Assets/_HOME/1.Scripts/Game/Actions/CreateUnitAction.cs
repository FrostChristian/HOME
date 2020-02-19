using UnityEngine;

namespace HOME.Game {

    [RequireComponent(typeof(Player))]
    public class CreateUnitAction : ActionBehavior {

        public GameObject Prefab;
        private PlayerSetupDefinition _player;
        private GameObject _spawnPoint;

        void Start() {
            if (Prefab.TryGetComponent(out Entity entity)) {// if we have a Entity on this
                _prefabEntity = entity;
            } else {
                Debug.Log("CreateBuildingAction! Start()  NO ENTITY ON PREFAB!");
            }

            _player = GetComponent<Player>().Info;
            if (GetComponent<Building>() != null) {

            _spawnPoint = GetComponent<Building>().spawnPoint;
            }
            if (GetComponent<Resource>() != null) {
            _spawnPoint = GetComponent<Resource>().spawnPoint;

            }
        }

        public override System.Action GetClickAction() { // add action to button
            if (_spawnPoint.transform == null) {
                Debug.Log("NO SPAWN");
                return delegate () {
                };
            } else {
                return delegate () {
                    //_spawnPoint = GetComponent<Building>().spawnPoint;
                    if (_player.isAi) {
                        GameObject go = Instantiate(Prefab, _spawnPoint.transform.position, Quaternion.identity, _player.playerLocation.transform);//rotation default
                        go.GetComponent<Player>().Info = _player;
                        go.GetComponent<Entity>().madeBy = GetComponent<Entity>();
                    } else {
                        if (_prefabEntity.CheckCost(false)) { // check for resources
                            GameObject go = Instantiate(Prefab, _spawnPoint.transform.position, Quaternion.identity, _player.playerLocation.transform);//rotation default
                            go.GetComponent<Player>().Info.isAi = false;
                            go.AddComponent<ShowActionButton>();
                            go.GetComponent<Player>().Info = _player;
                            go.GetComponent<Entity>().madeBy = GetComponent<Entity>();

                        } else {
                            Debug.Log("CreateBuildingAction! GetClickAction() Sirious error!");
                        }
                    }
                };
            }
        }
    }
}

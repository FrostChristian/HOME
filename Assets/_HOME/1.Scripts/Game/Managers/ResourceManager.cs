using System;
using System.Collections.Generic;
using UnityEngine;
using HOME.Data;

namespace HOME.Game {

    public class ResourceManager : MonoBehaviour {

        public static ResourceManager Instance;

        [SerializeField] public List<GatherAI> selectedGathererAIList = default;
        [SerializeField] public List<Transform> ironStorageTransform = new List<Transform>();
        [SerializeField] private Transform storageTransform = default;


        public List<Resource> resourceEntityList;

        // list for inspector visibility
        [SerializeField] private List<GameObject> Food = new List<GameObject>();
        [SerializeField] private List<GameObject> Iron = new List<GameObject>();
        [SerializeField] private List<GameObject> Alloy = new List<GameObject>();
        [SerializeField] private List<GameObject> IronOre = new List<GameObject>();
        [SerializeField] private List<GameObject> BauxiteOre = new List<GameObject>();

        public void Init(GameManager gameManager) {
            if (gameManager == null) {
                Debug.LogWarning("ResourceManager Init() No Gamemanager Found!");
                return;

            }

            Instance = this;
            resourceEntityList = new List<Resource>(); // create RE
            //resourceEntityList.Clear();       


            //storageTransform = GameObject.Find("Storage").transform; // find storage

            foreach (var iron in GameObject.FindGameObjectsWithTag("IronEntity")) {
                resourceEntityList.Add(iron.GetComponent<Resource>());
                Iron.Add(iron.gameObject);
            }

            foreach (var alloy in GameObject.FindGameObjectsWithTag("AlloyEntity")) {
                resourceEntityList.Add(alloy.GetComponent<Resource>());
                Alloy.Add(alloy.gameObject);

            }

            foreach (var food in GameObject.FindGameObjectsWithTag("FoodEntity")) {
                resourceEntityList.Add(food.GetComponent<Resource>());
                Food.Add(food.gameObject);
            }

            foreach (var ironOre in GameObject.FindGameObjectsWithTag("IronOreEntity")) {
                resourceEntityList.Add(ironOre.GetComponent<Resource>());
                IronOre.Add(ironOre.gameObject);
            }

            foreach (var bauxiteOre in GameObject.FindGameObjectsWithTag("BauxiteOreEntity")) {
                resourceEntityList.Add(bauxiteOre.GetComponent<Resource>());
                BauxiteOre.Add(bauxiteOre.gameObject);
            }

            Entity.OnStorageEntityClicked += Storage_OnStorageEntityClicked; // Get clicked resourceEntity
            Resource.OnResourceEntityClicked += ResourceEntity_OnResourceEntityClicked; // Get clicked resourceEntity
            MouseManager.OnGathererClicked += GathererAI_OnGathererClicked; //get clicked gatherer AI
        }

        private void GathererAI_OnGathererClicked(object sender, EventArgs e) {
            //Debug.Log("Gatherer Clicked! : "+sender);
            GatherAI gathererAI = sender as GatherAI;
            selectedGathererAIList.Add(gathererAI);
        }


        private void ResourceEntity_OnResourceEntityClicked(object sender, EventArgs e) {// function called when clicked on node
            Debug.Log("Resource Clicked! : " + sender);
            Resource resourceEntity = sender as Resource; // cast obj to ResEnt.
            if (selectedGathererAIList.Count != 0) {

                foreach (var gatherer in selectedGathererAIList) {
                    gatherer.SetResourceEntity(resourceEntity);
                }
                Debug.Log(" Selected gatherer AI On Its Way");
            } else {
                Debug.Log(" Selected gatherer AI is null, no one is selected");
            }
        }

        private void Storage_OnStorageEntityClicked(object sender, EventArgs e) {// function called when clicked on node
            Debug.Log("Storage Clicked! : " + sender);
            Transform storageEntity = sender as Transform; // cast obj to ResEnt.
            if (selectedGathererAIList.Count != 0) {
                foreach (var gatherer in selectedGathererAIList) {
                    gatherer.MovedToStorage(storageEntity);
                }
            }
        }

        private Resource GetResourceEntity() {
            List<Resource> tmpResourceNodeList = new List<Resource>(resourceEntityList); // clone the resource node list
            for (int i = 0; i < tmpResourceNodeList.Count; i++) {
                if (!tmpResourceNodeList[i].HasResources()) { //if it has no resources
                    tmpResourceNodeList.RemoveAt(i); // remove the entity as a targate for collection
                    i--;
                }
            }

            if (tmpResourceNodeList.Count > 0) { // if not emty return target
                return tmpResourceNodeList[UnityEngine.Random.Range(0, tmpResourceNodeList.Count)]; // return random entity if list is bigger than 0
            } else {
                return null; // if emty == no resources there!
            }
        }

        public static Resource GetResourceEntity_Static() {
            return Instance.GetResourceEntity();
        }

        private Resource GetResourceEntityNearPosition(Vector3 position, DataManager.ResourceType resourceType) {
            float maxDistance = 20f; // from position
            List<Resource> tmpResourceEntityList = new List<Resource>(resourceEntityList); // clone the resource node list
            for (int i = 0; i < tmpResourceEntityList.Count; i++) {
                if (!tmpResourceEntityList[i].HasResources() ||
                    Vector3.Distance(position, tmpResourceEntityList[i].GetPosition()) > maxDistance ||
                    tmpResourceEntityList[i].GetResourceType() != resourceType) { //if it has no resources, or to faar away, or other resource than specified
                    tmpResourceEntityList.RemoveAt(i); // remove the entity as a targate for collection
                    i--;
                }
            }
            if (tmpResourceEntityList.Count > 0) { // if not emty return target
                return tmpResourceEntityList[UnityEngine.Random.Range(0, tmpResourceEntityList.Count)]; // return random entity if list is bigger than 0
            } else {
                return null; // if emty == no resources there!
            }
        }

        public static Resource GetResourceEntityNearPosition_Static(Vector3 position, DataManager.ResourceType resourceType) {
            return Instance.GetResourceEntityNearPosition(position, resourceType);
        }

        public void SetStorageEntity(Transform storageTransform) {
            this.storageTransform = storageTransform;
        }

        private Transform GetStorage() {
            // foreach smelter get position
            // return closest position
            if (ironStorageTransform.Count > 0) { // if not emty return target
                storageTransform = ironStorageTransform[UnityEngine.Random.Range(0, ironStorageTransform.Count)]; // return random entity if list is bigger than 0
                return storageTransform;
            } else {
                return null; // if emty == no resources there!
            }
        }

        public static Transform GetStorage_Static() {
            return Instance.GetStorage();
        }
    }
}

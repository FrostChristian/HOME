using UnityEngine;

namespace HOME.Game {

    public class Player : MonoBehaviour {

    public PlayerSetupDefinition Info;//Stores all Player Info;
    public static PlayerSetupDefinition humanPlayer; 

    void Start() {
        Info.ActiveUnits.Add(this.gameObject);
    }
    void OnDestroy() {
        Info.ActiveUnits.Remove(this.gameObject);
    }
}
}

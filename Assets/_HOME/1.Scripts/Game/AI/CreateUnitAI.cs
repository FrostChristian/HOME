using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOME.Game {

    public class CreateUnitAI : AIBehavior {

        public int unitsPerBase = 5; //how many units are allowed per base
        public float cost = 25; //Cost of the unit
        private AISupport support; // get acces to support

        public override float GetWeight() {
            if (support == null) { // null ref check + add obj
                support = AISupport.GetSupport(gameObject);
            }
            if (support.Player.playerCredits < cost) { //cost check
                Debug.Log(""+ support.Player.playerCredits);                            //Debug.Log("Return: No money = no Drones");
                return 0;
            }
            var drones = support.AIUnits.Count;
            var bases = support.AIBases.Count;

            if (bases == 0) { //if no base, cant build drone!
                              //Debug.Log("Return: No base = no Drones");
                return 0;
            }
            if (drones >= bases * unitsPerBase) { //more drones than bases can support
                                                  //Debug.Log("Return: more drones than can be supportet");
                return 0;
            }
            return 1; //build a drone
        }

        public override void Execute() {
            Debug.Log("CreateUnitAI: " + support.Player.playerName + " is creating a unit.");

            List<GameObject> bases = support.AIBases; //select base to build from at random
            int index = Random.Range(0, bases.Count);
            GameObject commandBase = bases[index]; //select random from index
            commandBase?.GetComponent<CreateUnitAction>().GetClickAction()(); // get the actual button blickaction ()()
        }
    }
}

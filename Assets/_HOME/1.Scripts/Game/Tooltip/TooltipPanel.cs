/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

/// 
/// Added find child transform by name func
/// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOME.Game {

    public class TooltipPanel : MonoBehaviour {

        private float attackCooldown;

        private void Start() {
            Tooltip.AddTooltip(ReturnChildTransformByName(gameObject, "SelDestroy"), "Destroy");
            Tooltip.AddTooltip(ReturnChildTransformByName(transform.gameObject, "CloseBtn"), "Close");
            //Tooltip.AddTooltip(ReturnChildTransformByName(transform.gameObject, "Stop/StartRepBtn"), "Stop/StartRepBtn");
            Tooltip.AddTooltip(ReturnChildTransformByName(transform.gameObject, "FlyAwayBtn"), "Repair your ship and fly away!");
            Tooltip.AddTooltip(ReturnChildTransformByName(transform.gameObject, "SummaryBtn"), "SummaryBtn");
            Tooltip.AddTooltip(ReturnChildTransformByName(transform.gameObject, "UpgradeBtn"), "Upgrades");
            Tooltip.AddTooltip(ReturnChildTransformByName(transform.gameObject, "MenuBtn"), "Menu");            
        }

        private Transform ReturnChildTransformByName(GameObject go, string targetChild) {
            Transform[] allChildren = go.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren) {
                if (child.gameObject.name == targetChild) {
                return child.transform;
                }
            }
            return null;
        }
    }
}

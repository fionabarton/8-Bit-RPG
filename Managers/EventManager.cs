using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script is NOT referenced in any scene (attached to a game object)
 * 
 * It's referenced in the following scripts:
 * ActivateOnButtonPress, EventManager, ShopkeeperTrigger, ShopMenu
 */
public class EventManager : MonoBehaviour {
    public delegate void ReactivateShopkeeperTriggerAction();
    public static event ReactivateShopkeeperTriggerAction OnShopScreenDeactivated;

    public static void ShopScreenDeactivated() {
        OnShopScreenDeactivated?.Invoke();
    }
}
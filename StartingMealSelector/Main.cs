using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace KitchenStartingMealSelector {

    [BepInProcess("PlateUp.exe")]
    [BepInPlugin(MOD_ID, MOD_NAME, MOD_VERSION)]
    public class Mod : BaseUnityPlugin {

        public const string MOD_ID = "StartingMealSelector";
        public const string MOD_NAME = "Starting Meal Selector";
        public const string MOD_VERSION = "0.0.1";

        public static int selectedStartingDish = 0;
        public static List<int> loadedAvailableMenuOptions = null;

        public void Awake() {
            Debug.LogWarning($"BepInEx mod loaded: {MOD_NAME} {MOD_VERSION}");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MOD_ID);
        }
    }
}

using KitchenMods;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using HarmonyLib;

namespace KitchenStartingMealSelector {

    public class Main : IModInitializer {

        public const string MOD_ID = "blargle.StartingMealSelector";
        public const string MOD_NAME = "Starting Meal Selector";
        public const string MOD_VERSION = "0.2.1";

        public static int selectedStartingDish = 0;
        public static bool randomizeOnce = false;
        public static bool refreshOptions = false;
        public static List<int> loadedAvailableMenuOptions = new List<int>();
        public static List<string> loadedAvailableMenuOptionNames = new List<string>();
        public static Dictionary<int, string> availableMenuOptions = new Dictionary<int, string>();

        public void PostActivate(Mod mod) {
            Debug.Log($"[{MOD_ID}] v{MOD_VERSION} initialized");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MOD_ID);
        }

        public void PreInject() { 
            SmsPreferences.registerPreferences();
        }

        public void PostInject() { }
    }
}

using KitchenMods;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using System.Runtime.CompilerServices;

namespace KitchenStartingMealSelector {

    public class Main : IModInitializer {

        public const string MOD_ID = "blargle.StartingMealSelector";
        public const string MOD_NAME = "Starting Meal Selector";
        public static readonly string MOD_VERSION = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.ToString();

        public static int selectedStartingDish = 0;
        public static bool randomizeOnce = false;
        public static bool refreshOptions = false;
        public static List<int> loadedAvailableMenuOptions = new List<int>();
        public static List<string> loadedAvailableMenuOptionNames = new List<string>();
        public static Dictionary<int, string> availableMenuOptions = new Dictionary<int, string>();
        public static string selectedSeed = "";
        public static int selectedSetting = 0;
        public static Dictionary<int, string> availableSettingOptions = new Dictionary<int, string>();

        public void PostActivate(Mod mod) {
            Log($"v{MOD_VERSION} initialized");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), MOD_ID);
        }

        public void PreInject() { 
            SmsPreferences.registerPreferences();
        }

        public void PostInject() { }

        public static void Log(object message, [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null) {
            Debug.Log($"[{MOD_ID}] [{caller}({callingFilePath}:{lineNumber})] {message}");
        }
    }
}

using Kitchen;
using KitchenLib;
using KitchenLib.Event;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace KitchenStartingMealSelector {

    public class Mod : BaseMod {

        public const string MOD_ID = "blargle.StartingMealSelector";
        public const string MOD_NAME = "Starting Meal Selector";
        public const string MOD_VERSION = "0.0.9";

        public static int selectedStartingDish = 0;
        public static bool randomizeOnce = false;
        public static bool refreshOptions = false;
        public static List<int> loadedAvailableMenuOptions = new List<int>();
        public static List<string> loadedAvailableMenuOptionNames = new List<string>();

        public Mod() : base(MOD_ID, MOD_NAME, "blargle", MOD_VERSION, ">=1.1.5", Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise() {
            Debug.Log($"[{MOD_ID}] v{MOD_VERSION} initialized");
            initPauseMenu();
        }

        private void initPauseMenu() {
            ModsPreferencesMenu<PauseMenuAction>.RegisterMenu(MOD_NAME, typeof(StartingMealSelectorMenu<PauseMenuAction>), typeof(PauseMenuAction));
            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) => {
                args.Menus.Add(typeof(StartingMealSelectorMenu<PauseMenuAction>), new StartingMealSelectorMenu<PauseMenuAction>(args.Container, args.Module_list));
            };
        }
    }
}

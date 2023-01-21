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
        public const string MOD_VERSION = "0.0.6";

        public static bool isRegistered = false;
        public static int selectedStartingDish = 0;
        public static List<int> loadedAvailableMenuOptions = new List<int>();
        public static List<string> loadedAvailableMenuOptionNames = new List<string>();

        public Mod() : base(MOD_ID, MOD_NAME, "blargle", MOD_VERSION, "1.1.3", Assembly.GetExecutingAssembly()) { }

        protected override void Initialise() {
            base.Initialise();
            if (!isRegistered) {
                Debug.Log($"[{MOD_ID}] v{MOD_VERSION} initialized");
                initPauseMenu();
                isRegistered = true;
            }
        }

        private void initPauseMenu() {
            ModsPreferencesMenu<PauseMenuAction>.RegisterMenu(MOD_NAME, typeof(StartingMealSelectorMenu<PauseMenuAction>), typeof(PauseMenuAction));
            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) => {
                args.Menus.Add(typeof(StartingMealSelectorMenu<PauseMenuAction>), new StartingMealSelectorMenu<PauseMenuAction>(args.Container, args.Module_list));
            };
        }
    }
}

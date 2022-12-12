using HarmonyLib;
using Kitchen;
using KitchenLib;
using KitchenLib.Event;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace KitchenStartingMealSelector {

    public class Mod : BaseMod {

        public const string MOD_ID = "blargle.StartingMealSelector";
        public const string MOD_NAME = "Starting Meal Selector";
        public const string MOD_VERSION = "0.0.3";

        public static bool isRegistered = false;
        public static int selectedStartingDish = 0;
        public static List<int> loadedAvailableMenuOptions = null;

        public Mod() : base(MOD_ID, MOD_NAME, "blargle", MOD_VERSION, "1.1.2", Assembly.GetExecutingAssembly()) { }

        protected override void Initialise() {
            base.Initialise();
            if (!isRegistered) {
                Debug.Log($"{MOD_ID} v{MOD_VERSION}: initialized");
                initMainMenu();
                initPauseMenu();
                isRegistered = true;
            } else {
                Debug.Log($"{MOD_ID} v{MOD_VERSION}: skipping re-creating menus");
            }
        }

        protected override void OnUpdate() {
        }

        private void initMainMenu() {
            Events.PreferenceMenu_MainMenu_SetupEvent += (s, args) => {
                Type type = args.instance.GetType().GetGenericArguments()[0];
                args.mInfo.Invoke(args.instance, new object[] { MOD_NAME, typeof(StartingMealSelectorMenu<>).MakeGenericType(type), false });
            };
            Events.PreferenceMenu_MainMenu_CreateSubmenusEvent += (s, args) => {
                args.Menus.Add(typeof(StartingMealSelectorMenu<MainMenuAction>), new StartingMealSelectorMenu<MainMenuAction>(args.Container, args.Module_list));
            };
        }

        private void initPauseMenu() {
            Events.PreferenceMenu_PauseMenu_SetupEvent += (s, args) => {
                Type type = args.instance.GetType().GetGenericArguments()[0];
                args.mInfo.Invoke(args.instance, new object[] { MOD_NAME, typeof(StartingMealSelectorMenu<>).MakeGenericType(type), false });
            };
            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) => {
                args.Menus.Add(typeof(StartingMealSelectorMenu<PauseMenuAction>), new StartingMealSelectorMenu<PauseMenuAction>(args.Container, args.Module_list));
            };
        }
    }
}

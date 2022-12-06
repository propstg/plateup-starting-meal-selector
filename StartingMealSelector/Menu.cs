using UnityEngine;
using Kitchen.Modules;
using Kitchen;
using System.Collections.Generic;
using HarmonyLib;
using BepInEx.Logging;
using System.Reflection;

namespace KitchenStartingMealSelector {

    public class StartingMealSelectorMenu : Menu<PauseMenuAction> {

        private static readonly Dictionary<int, string> allOptions = new Dictionary<int, string>() {
            {-2075899, "Breakfast"},
            {-1778969928, "Burgers"},
            {1626323920, "Hotdogs"},
            {1743900205, "Fish"},
            {-133939790, "Pie"},
            {1356267749, "Salad"},
            {-959076098, "Steak"},
            {-1653221873, "Stir Fry"},
            {550743424, "Pizza"}
        };

        public StartingMealSelectorMenu(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id) {
            List<int> dishValues = new List<int>();
            List<string> dishLabels = new List<string>();

            foreach (var entry in allOptions) {
                if (Mod.loadedAvailableMenuOptions.Contains(entry.Key)) {
                    dishValues.Add(entry.Key);
                    dishLabels.Add(entry.Value);
                }
            }

            AddLabel("What dish would you like to force to appear?");
            Add(new Option<int>(dishValues, Mod.selectedStartingDish, dishLabels))
                .OnChanged += delegate (object _, int value) {
                    Mod.selectedStartingDish = value;
                };

            New<SpacerElement>();
            New<SpacerElement>();

            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });
        }
    }

    [HarmonyPatch(typeof(MainMenu), "Setup")]
    class MainMenu_Patch {

        private static ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource($"{Mod.MOD_NAME} MainMenu_Patch");

        public static bool Prefix(MainMenu __instance) {
            log.LogInfo("In main menu patch");
            MethodInfo addSubmenu = __instance.GetType().GetMethod("AddSubmenuButton", BindingFlags.NonPublic | BindingFlags.Instance);
            addSubmenu.Invoke(__instance, new object[] { Mod.MOD_NAME, typeof(StartingMealSelectorMenu), false });
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerPauseView), "SetupMenus")]
    class PauseMenu_Patch {

        private static ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource($"{Mod.MOD_NAME} PauseMenu_Patch");

        public static bool Prefix(PlayerPauseView __instance) {
            log.LogInfo("In pause menu patch");
            ModuleList moduleList = (ModuleList)__instance.GetType().GetField("ModuleList", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            MethodInfo addMenu = __instance.GetType().GetMethod("AddMenu", BindingFlags.NonPublic | BindingFlags.Instance);
            addMenu.Invoke(__instance, new object[] { typeof(StartingMealSelectorMenu), new StartingMealSelectorMenu(__instance.ButtonContainer, moduleList) });
            return true;
        }
    }
}

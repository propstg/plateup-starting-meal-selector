using UnityEngine;
using Kitchen.Modules;
using Kitchen;
using HarmonyLib;
using System.Reflection;
using System.Linq;

namespace KitchenStartingMealSelector {

    public class StartingMealSelectorMenu : Menu<PauseMenuAction> {

        private int selectedOption = 0;

        public StartingMealSelectorMenu(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id) {
            selectedOption = 0;

            AddLabel("What dish would you like to force to appear?");
            Add(new Option<int>(Main.availableMenuOptions.Keys.ToList(), 0, Main.availableMenuOptions.Values.ToList()))
                .OnChanged += delegate (object _, int value) {
                    selectedOption = value;
                };

            New<SpacerElement>();

            AddInfo("Choose 'Surprise Me' to have a dish randomly selected for you. (Note: The test kitchen in the lobby will be disabled while this is active).");

            New<SpacerElement>();
            AddInfo("This menu only shows options that you have unlocked and is not currently intended to unlock meals.");

            AddButton("Refresh Options", delegate {
                Main.selectedStartingDish = 0;
                Main.refreshOptions = true;
                RequestPreviousMenu();
            });

            New<SpacerElement>();
            New<SpacerElement>();
            AddButton(Localisation["MENU_APPLY_SETTINGS"], delegate {
                int previousSelection = Main.selectedStartingDish;
                Main.selectedStartingDish = selectedOption;
                if (selectedOption == 0 && previousSelection != -1) {
                    Main.randomizeOnce = true;
                }
                RequestPreviousMenu();
            });
            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });
        }

    }

    [HarmonyPatch(typeof(MainMenu), "Setup")]
    class MainMenu_Patch {

        public static bool Prefix(MainMenu __instance) {
            MethodInfo addSubmenu = __instance.GetType().GetMethod("AddSubmenuButton", BindingFlags.NonPublic | BindingFlags.Instance);
            addSubmenu.Invoke(__instance, new object[] { Main.MOD_NAME, typeof(StartingMealSelectorMenu), false });
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerPauseView), "SetupMenus")]
    class PauseMenu_Patch {

        public static bool Prefix(PlayerPauseView __instance) {
            ModuleList moduleList = (ModuleList)__instance.GetType().GetField("ModuleList", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            MethodInfo addMenu = __instance.GetType().GetMethod("AddMenu", BindingFlags.NonPublic | BindingFlags.Instance);
            addMenu.Invoke(__instance, new object[] { typeof(StartingMealSelectorMenu), new StartingMealSelectorMenu(__instance.ButtonContainer, moduleList) });
            return true;
        }
    }
}

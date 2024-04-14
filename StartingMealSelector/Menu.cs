using UnityEngine;
using Kitchen.Modules;
using Kitchen;
using HarmonyLib;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace KitchenStartingMealSelector {

    public class StartingMealSelectorMenu : Menu<PauseMenuAction> {

        private int selectedOption = 0;

        public StartingMealSelectorMenu(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id) {
            selectedOption = 0;

            ButtonElement startWithThisSelectedButton = null;
            AddLabel("What dish would you like to force to appear?");
            Add(new Option<int>(Main.availableMenuOptions.Keys.ToList(), 0, Main.availableMenuOptions.Values.ToList()))
                .OnChanged += delegate (object _, int value) {
                    selectedOption = value;
                    startWithThisSelectedButton.SetSelectable(selectedOption != 0 && selectedOption != -1);
                };
            AddInfo("Choose 'Surprise Me' to have a dish randomly selected for you. (Note: The test kitchen in the lobby will be disabled while this is active).");
            AddButton("Choose", delegate {
                int previousSelection = Main.selectedStartingDish;
                Main.selectedStartingDish = selectedOption;
                if (selectedOption == 0 && previousSelection != -1) {
                    Main.randomizeOnce = true;
                }
                RequestPreviousMenu();
            });

            InfoBoxElement startingGameWithElement = null;
            ButtonElement clearStartingWithButton = null;
            startWithThisSelectedButton = AddButton("Always Start Game with This Dish", delegate {
                SmsPreferences.setStartWithMeal(selectedOption);
                clearStartingWithButton.SetSelectable(true);
                updateStartGameWithLabel(startingGameWithElement);
            });
            startWithThisSelectedButton.SetSelectable(false);

            startingGameWithElement = AddInfo("");
            clearStartingWithButton = AddButton("Clear Start Game with Dish", delegate {
                SmsPreferences.setStartWithMeal(0);
                clearStartingWithButton.SetSelectable(false);
                updateStartGameWithLabel(startingGameWithElement);
            });

            addSeedSection();

            New<SpacerElement>();
            AddButton("Refresh Options", delegate {
                Main.selectedStartingDish = 0;
                Main.refreshOptions = true;
                RequestPreviousMenu();
            });
            AddInfo("This menu only shows options that you have unlocked and is not currently intended to unlock meals.");

            New<SpacerElement>();
            New<SpacerElement>();
            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });

            updateStartGameWithLabel(startingGameWithElement);
            clearStartingWithButton.SetSelectable(SmsPreferences.getStartWithMeal() != 0);
        }

        private void addSeedSection() {
            ButtonElement startWithThisSeedSelectedButton = null;
            InfoBoxElement startingGameWithSeedElement = null;
            ButtonElement clearStartingWithSeedButton = null;
            startWithThisSeedSelectedButton = AddButton($"Always Start Game with Seed {Main.selectedSeed.ToUpper()} and setting {Main.availableSettingOptions.GetValueOrDefault(Main.selectedSetting, "Unknown")}", delegate {
                SmsPreferences.setStartWithSeed(Main.selectedSeed);
                SmsPreferences.setStartWithSetting(Main.selectedSetting);
                clearStartingWithSeedButton.SetSelectable(true);
                updateStartGameWithSeedLabel(startingGameWithSeedElement);
            });
            startWithThisSeedSelectedButton.SetSelectable(Main.selectedSeed != "");

            startingGameWithSeedElement = AddInfo("");
            clearStartingWithSeedButton = AddButton("Clear Start Game with Seed", delegate {
                SmsPreferences.setStartWithSeed("");
                clearStartingWithSeedButton.SetSelectable(false);
                updateStartGameWithSeedLabel(startingGameWithSeedElement);
            });

            updateStartGameWithSeedLabel(startingGameWithSeedElement);
            clearStartingWithSeedButton.SetSelectable(SmsPreferences.getStartWithSeed() != "");
        }


        private void updateStartGameWithLabel(InfoBoxElement label) {
            if (SmsPreferences.getStartWithMeal() == 0) {
                label.SetLabel("Game will not start with any meal selected automatically");
            } else {
                label.SetLabel($"Game will start with {Main.availableMenuOptions.GetValueOrDefault(SmsPreferences.getStartWithMeal(), "Unknown")} selected automatically.");
            }
        }

        private void updateStartGameWithSeedLabel(InfoBoxElement label) {
            if (SmsPreferences.getStartWithSeed() == "") {
                label.SetLabel("Game will not start with any seed selected automatically");
            } else {
                label.SetLabel($"Game will start with {SmsPreferences.getStartWithSeed().ToUpper()} and setting {Main.availableSettingOptions.GetValueOrDefault(SmsPreferences.getStartWithSetting(), "Unknown")} selected automatically.");
            }
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

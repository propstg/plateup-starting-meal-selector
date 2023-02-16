using UnityEngine;
using Kitchen.Modules;
using Kitchen;
using System.Collections.Generic;
using KitchenLib;
using KitchenLib.Utils;
using KitchenData;

namespace KitchenStartingMealSelector {

    public class StartingMealSelectorMenu<T> : KLMenu<T> {

        public StartingMealSelectorMenu(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id) {
            AddLabel("What dish would you like to force to appear?");
            Add(new Option<int>(Mod.loadedAvailableMenuOptions, 0, Mod.loadedAvailableMenuOptionNames))
                .OnChanged += delegate (object _, int value) {
                    Mod.selectedStartingDish = value;
                    if (value == 0) {
                        Mod.randomizeOnce = true;
                    }
                };

            New<SpacerElement>();

            AddInfo("Choose 'Surprise Me' to have a dish randomly selected for you. (Note: The test kitchen in the lobby will be disabled while this is active.)");

            New<SpacerElement>();
            AddInfo("This menu only shows options that you have unlocked and is not currently intended to unlock meals.");

            New<SpacerElement>();
            New<SpacerElement>();
            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });
        }
    }
}

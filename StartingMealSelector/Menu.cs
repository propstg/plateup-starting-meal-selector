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

        private List<int> dishValues = new List<int> { 0, 536093200, 367917843 };
        private List<string> dishLabels = new List<string> { "Random", "Nut Roast", "Dumplings" };

        public override void Setup(int player_id) {
            addInGameDishesThatUserHasAccessTo();

            AddLabel("What dish would you like to force to appear?");
            Add(new Option<int>(dishValues, Mod.selectedStartingDish, dishLabels))
                .OnChanged += delegate (object _, int value) {
                    Mod.selectedStartingDish = value;
                };

            New<SpacerElement>();

            AddInfo("Choose 'Random' for randomly selected meals on future lobby visits.");

            New<SpacerElement>();
            AddInfo("This menu only shows options that you have unlocked and is not currently intended to unlock meals.");

            New<SpacerElement>();
            New<SpacerElement>();
            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });
        }

        private void addInGameDishesThatUserHasAccessTo() {
            foreach (var id in Mod.loadedAvailableMenuOptions) {
                dishValues.Add(id);
                GameDataObject gameDataObject = GDOUtils.GetExistingGDO(id);
                if (gameDataObject != null && gameDataObject.name != null) {
                    string[] name = gameDataObject.name.Split(' ');
                    dishLabels.Add(name[0]);
                } else {
                    dishLabels.Add("" + id);
                }
            }
        }
    }
}

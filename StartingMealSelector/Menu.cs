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
            List<int> dishValues = new List<int>();
            List<string> dishLabels = new List<string>();

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
}

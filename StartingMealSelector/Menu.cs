using UnityEngine;
using Kitchen.Modules;
using Kitchen;
using System.Collections.Generic;
using KitchenLib;

namespace KitchenStartingMealSelector {

    public class StartingMealSelectorMenu<T> : KLMenu<T> {

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
}

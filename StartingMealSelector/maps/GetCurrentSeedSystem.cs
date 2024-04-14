using Kitchen;
using KitchenMods;
using KitchenStartingMealSelector;
using Unity.Entities;

namespace StartingMealSelector.maps {

    public class GetCurrentSeedSystem : FranchiseSystem, IModSystem {

        protected override void Initialise() {
            base.Initialise();
        }

        protected override void OnUpdate() {
            if (RequireEntity<SSelectedLayoutPedestal>(out Entity selectedLayout)) {
                if (GetComponentOfHeld<CSetting>(selectedLayout, out CSetting setting)) {
                    Main.selectedSeed = setting.FixedSeed.StrValue;
                    Main.selectedSetting = setting.RestaurantSetting;
                }
            }
        }
    }
}

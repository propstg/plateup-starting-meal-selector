using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace KitchenStartingMealSelector {

    public class SelectMealAtStartOfGameSystem : FranchiseFirstFrameSystem, IModSystem {

        protected override void Initialise() {
            base.Initialise();
        }

        protected override void OnUpdate() {
            int startWithMeal = SmsPreferences.getStartWithMeal();
            if (startWithMeal != 0) {
                Main.selectedStartingDish = startWithMeal;
            }
        }
    }
}

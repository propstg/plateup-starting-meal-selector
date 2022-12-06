using Kitchen;
using Unity.Entities;
using UnityEngine;

namespace KitchenStartingMealSelector {

    public class DishPaperOverrideSystem : GameSystemBase {

        private EntityQuery dishPaperQuery;

        protected override void Initialise() {
            base.Initialise();
            dishPaperQuery = GetEntityQuery(new QueryHelper().All(typeof(CDishChoice)));
            Debug.LogWarning($"{Mod.MOD_ID}: DishPaperChangerSystem initialized.");
        }

        protected override void OnUpdate() {
            if (Mod.selectedStartingDish == 0) {
                return;
            }

            var items = dishPaperQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
            foreach (var dish in items) {
                if (dish != null) {
                    CDishChoice cDishChoice = EntityManager.GetComponentData<CDishChoice>(dish);
                    cDishChoice.Dish = Mod.selectedStartingDish;
                    EntityManager.SetComponentData<CDishChoice>(dish, cDishChoice);
                }
            }
            items.Dispose();
        }
    }
}

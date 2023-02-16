using Kitchen;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace KitchenStartingMealSelector {

    public class DishPaperOverrideSystem : FranchiseSystem, IModSystem {

        private EntityQuery dishPaperQuery;

        protected override void Initialise() {
            base.Initialise();
            dishPaperQuery = GetEntityQuery(new QueryHelper().All(typeof(CDishChoice)));
            Debug.Log($"[{Mod.MOD_ID}] DishPaperChangerSystem initialized.");
        }

        protected override void OnUpdate() {
            if (Mod.selectedStartingDish == 0 && !Mod.randomizeOnce) {
                return;
            }

            var items = dishPaperQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
            foreach (var dish in items) {
                if (dish != null) {
                    CDishChoice cDishChoice = EntityManager.GetComponentData<CDishChoice>(dish);

                    int dishId = Mod.selectedStartingDish;
                    if (dishId == 0 || dishId == -1) {
                        dishId = Mod.loadedAvailableMenuOptions[Random.Range(2, Mod.loadedAvailableMenuOptions.Count)];
                    }
                    cDishChoice.Dish = dishId;

                    EntityManager.SetComponentData<CDishChoice>(dish, cDishChoice);
                }
            }
            items.Dispose();
            Mod.randomizeOnce = false;
        }
    }
}

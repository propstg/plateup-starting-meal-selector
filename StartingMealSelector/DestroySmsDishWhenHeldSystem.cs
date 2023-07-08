using Kitchen;
using KitchenMods;
using Unity.Entities;
using UnityEngine;

namespace KitchenStartingMealSelector {

    public class DestroySmsDishWhenHeldSystem : FranchiseSystem, IModSystem {

        private EntityQuery query;

        protected override void Initialise() {
            base.Initialise();
            query = GetEntityQuery((ComponentType)typeof(CStartingMealSelectorDish), (ComponentType)typeof(CHeldBy));
            RequireForUpdate(query);
        }

        protected override void OnUpdate() {
            var items = query.ToEntityArray(Unity.Collections.Allocator.TempJob);
            EntityContext entityContext = new EntityContext(EntityManager);
            Entity dishPedestal = entityContext.GetEntity<SDishPedestal>();
            foreach (var dish in items) {
                if (dish != null) {
                    CHeldBy heldBy;
                    if (Require<CHeldBy>(dish, out heldBy)) {
                        if (heldBy.Holder != dishPedestal) {
                            if (Main.selectedStartingDish == -1) {
                                Main.selectedStartingDish = Main.loadedAvailableMenuOptions[Random.Range(0, Main.loadedAvailableMenuOptions.Count)];
                            } else {
                                entityContext.Destroy(dish);
                            }
                        }
                    }
                }
            }
            items.Dispose();
        }
    }
}

using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace KitchenStartingMealSelector {

    [UpdateBefore(typeof(RebuildKitchen))]
    public class DoNotOverwhelmTheFranchiseKitchenSystem : FranchiseSystem, IModSystem {

        private EntityQuery rebuildRequests;

        protected override void Initialise() {
            base.Initialise();
            rebuildRequests = GetEntityQuery((ComponentType)typeof(RebuildKitchen.CRebuildKitchen));
            RequireForUpdate(rebuildRequests);
        }

        protected override void OnUpdate() {
            if (Main.selectedStartingDish == -1) {
                EntityManager.DestroyEntity(rebuildRequests);
            }
        }
    }
}

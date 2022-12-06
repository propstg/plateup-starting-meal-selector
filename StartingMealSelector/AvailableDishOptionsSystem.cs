using Kitchen;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace KitchenStartingMealSelector {

    public class AvailableDishOptionsSystem : GameSystemBase {

        private EntityQuery dishUpgradesQuery;

        protected override void Initialise() {
            base.Initialise();
            dishUpgradesQuery = GetEntityQuery((ComponentType)typeof(CDishUpgrade));
            Debug.LogWarning($"{Mod.MOD_ID}: AvailableDishOptionsSystem initialized.");
        }

        protected override void OnUpdate() {
            if (Mod.loadedAvailableMenuOptions != null && Mod.loadedAvailableMenuOptions.Count != 0) {
                return;
            }

            Mod.loadedAvailableMenuOptions = dishUpgradesQuery
                .ToComponentDataArray<CDishUpgrade>(Unity.Collections.Allocator.Temp)
                .ToList<CDishUpgrade>()
                .Select(item => item.DishID).ToList();

            Debug.LogWarning($"Found dish upgrades: {Mod.loadedAvailableMenuOptions.Select(item => item.ToString())}");
        }
    }
}

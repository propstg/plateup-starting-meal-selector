using Kitchen;
using KitchenMods;
using UnityEngine;

namespace KitchenStartingMealSelector {

    public class IndividualMealRerollSystem : ItemInteractionSystem, IModSystem {

        private CItemHolder itemHolder;
        private CDishChoice dishChoice;

        protected override InteractionType RequiredType => InteractionType.Act;

        protected override bool IsPossible(ref InteractionData data) {
            return Require<CItemHolder>(data.Target, out itemHolder) &&
                itemHolder.HeldItem != default &&
                Require<CDishChoice>(itemHolder.HeldItem, out dishChoice);
        }

        protected override void Perform(ref InteractionData data) {
            Mod.randomizeOnce = false;
            Mod.selectedStartingDish = 0;
            
            Debug.Log($"[{Mod.MOD_ID}] Rerolling individual dish. Current: {dishChoice.Dish}");
            dishChoice.Dish = Mod.loadedAvailableMenuOptions[Random.Range(2, Mod.loadedAvailableMenuOptions.Count)];
            Debug.Log($"[{Mod.MOD_ID}] New: {dishChoice.Dish}");
            SetComponent(itemHolder.HeldItem, dishChoice);
        }
    }
}

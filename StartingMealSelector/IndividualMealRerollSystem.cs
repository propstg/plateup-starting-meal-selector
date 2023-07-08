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
                Require<CDishChoice>(itemHolder.HeldItem, out dishChoice) &&
                dishChoice.Reason == FixedDishReason.None;
        }

        protected override void Perform(ref InteractionData data) {
            Main.randomizeOnce = false;
            Main.selectedStartingDish = 0;
            
            Debug.Log($"[{Main.MOD_ID}] Rerolling individual dish. Current: {dishChoice.Dish}");
            dishChoice.Dish = Main.loadedAvailableMenuOptions[Random.Range(0, Main.loadedAvailableMenuOptions.Count)];
            Debug.Log($"[{Main.MOD_ID}] New: {dishChoice.Dish}");
            SetComponent(itemHolder.HeldItem, dishChoice);
        }
    }
}

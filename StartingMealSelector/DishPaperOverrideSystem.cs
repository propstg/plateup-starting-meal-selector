using Kitchen;
using KitchenData;
using KitchenMods;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace KitchenStartingMealSelector {

    public class DishPaperOverrideSystem : FranchiseSystem, IModSystem {

        private EntityQuery dishPaperQuery;

        protected override void Initialise() {
            base.Initialise();
            dishPaperQuery = GetEntityQuery(new QueryHelper().All(typeof(CDishChoice), typeof(CItem)));
            Main.Log($"DishPaperChangerSystem initialized.");
        }


        protected override void OnUpdate() {
            if (Main.selectedStartingDish == 0 && !Main.randomizeOnce) {
                return;
            }

            EntityContext entityContext = new EntityContext(EntityManager);
            if (isFixedDishPedestalInUse(entityContext)) {
                return;
            }

            if (Main.selectedStartingDish == -1) {
                doRandomizeAll(entityContext);
            } else if (Main.selectedStartingDish == 0) {
                doRandomizeOnceIfNeeded();
            } else {
                doSelectDish(entityContext);
                Main.selectedStartingDish = 0;
            }

            Main.randomizeOnce = false;
        }


        private bool isFixedDishPedestalInUse(EntityContext entityContext) {
            Entity fixedDishPedestal = entityContext.GetEntity<SFixedDishPedestal>();
            return !entityContext.Has<CHideView>(fixedDishPedestal);
        }

        private void doRandomizeAll(EntityContext entityContext) {
            Entity dishPedestal = entityContext.GetEntity<SDishPedestal>();
            Entity paperOnHolder = getPaperOnPedestal(entityContext, dishPedestal);
            if (paperOnHolder != Entity.Null) {
                if (!isOurDishAlreadyOnPedestal(entityContext, paperOnHolder)) {
                    HolderHelpers.GoHome(EntityManager, paperOnHolder);
                    createNewDishPaperInPedestal(entityContext, dishPedestal, Main.loadedAvailableMenuOptions[0]);
                }
            } else {
                createNewDishPaperInPedestal(entityContext, dishPedestal, Main.loadedAvailableMenuOptions[0]);
            }
            randomizeDishes();
        }

        private void doRandomizeOnceIfNeeded() {
            if (Main.randomizeOnce) {
                randomizeDishes();
            }
        }

        private void randomizeDishes() {
            var randomizedDishes = Main.loadedAvailableMenuOptions.ToArray()
                .OrderBy(x => Random.Range(0, Main.loadedAvailableMenuOptions.Count)).ToList();

            var items = dishPaperQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
            int i = 0;
            foreach (var dish in items) {
                if (dish != null) {
                    CDishChoice cDishChoice = EntityManager.GetComponentData<CDishChoice>(dish);
                    if (cDishChoice.Reason != FixedDishReason.None || cDishChoice.Dish == 0) {
                        continue;
                    }

                    int dishId = Main.selectedStartingDish;
                    if (dishId == 0 || dishId == -1) {
                        dishId = randomizedDishes[i++];
                    }
                    cDishChoice.Dish = dishId;

                    EntityManager.SetComponentData<CDishChoice>(dish, cDishChoice);
                }
            }
            items.Dispose();
        }

        private void doSelectDish(EntityContext entityContext) {
            int dishId = Main.selectedStartingDish;

            if (dishId == 0 || dishId == -1) {
                return;
            }
            
            Entity dishPedestal = entityContext.GetEntity<SDishPedestal>();
            Entity paperOnHolder = getPaperOnPedestal(entityContext, dishPedestal);
            if (paperOnHolder != Entity.Null) {
                CDishChoice selectedDish;
                entityContext.Require<CDishChoice>(paperOnHolder, out selectedDish);
                if (isOurDishAlreadyOnPedestal(entityContext, paperOnHolder)) {
                    selectedDish.Dish = dishId;
                    EntityManager.SetComponentData<CDishChoice>(paperOnHolder, selectedDish);
                } else {
                    HolderHelpers.GoHome(EntityManager, paperOnHolder);
                    createNewDishPaperInPedestal(entityContext, dishPedestal, dishId);
                }
            } else {
                createNewDishPaperInPedestal(entityContext, dishPedestal, dishId);
            }
        }

        private Entity getPaperOnPedestal(EntityContext entityContext, Entity dishPedestal) {
            CItemHolder dishItemHolder;
            if (entityContext.Require<CItemHolder>(dishPedestal, out dishItemHolder)) {
                return dishItemHolder.HeldItem;
            }
            return Entity.Null;
        }

        private bool isOurDishAlreadyOnPedestal(EntityContext entityContext, Entity paperOnHolder) {
            return entityContext.Has<CStartingMealSelectorDish>(paperOnHolder);
        }

        private void createNewDishPaperInPedestal(EntityContext entityContext, Entity dishPedestal, int dishId) {
            Entity smsPaper = EntityManager.CreateEntity((ComponentType) typeof (CCreateItem), (ComponentType) typeof (RebuildKitchen.CNonKitchen), (ComponentType) typeof (CHeldBy), (ComponentType) typeof (CStartingMealSelectorDish));
            entityContext.Set<CCreateItem>(smsPaper, (CCreateItem) AssetReference.DishPaper);
            entityContext.Set<CHeldBy>(smsPaper, (CHeldBy) dishPedestal);
            entityContext.Set<CItemHolder>(dishPedestal, (CItemHolder) smsPaper);
            entityContext.Set<CDishChoice>(smsPaper, new CDishChoice() {
                Dish = dishId,
                Reason = FixedDishReason.None
            });
        }
    }
}

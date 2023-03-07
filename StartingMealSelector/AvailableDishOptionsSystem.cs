using Kitchen;
using KitchenData;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using KitchenLib.Utils;
using KitchenLib.References;
using UnityEngine.Rendering.Universal.Internal;
using System;

namespace KitchenStartingMealSelector {

    public class AvailableDishOptionsSystem : GameSystemBase {

        private EntityQuery dishUpgradesQuery;

        protected override void Initialise() {
            base.Initialise();
            dishUpgradesQuery = GetEntityQuery((ComponentType)typeof(CDishUpgrade));
            Debug.Log($"[{Mod.MOD_ID}] AvailableDishOptionsSystem initialized.");
        }

        protected override void OnUpdate() {
            if (!(Has<CSceneFirstFrame>() || Mod.refreshOptions)) {
                return;
            }

            Debug.LogWarning($"[{Mod.MOD_ID}] Loading dish upgrades...");

            Mod.loadedAvailableMenuOptions.Clear();
            Mod.loadedAvailableMenuOptionNames.Clear();

            addIdNamePair(-1, "Surprise me");
            addIdNamePair(0, "Random");
            addAlwaysAvailableOption();
            addOptionsThatUserHasUnlocked();
            addIdNamePair(DishReferences.NutRoastBase, "Nut Roast");
            
            Debug.LogWarning($"[{Mod.MOD_ID}] Found dish upgrades: {string.Join(", ", Mod.loadedAvailableMenuOptions.Select(item => item.ToString()))}");
            Mod.refreshOptions = false;
        }

        private void addIdNamePair(int id, string name) {
            Mod.loadedAvailableMenuOptions.Add(id);
            Mod.loadedAvailableMenuOptionNames.Add(name);
        }

        private void addAlwaysAvailableOption() {
            addDishOption(AssetReference.AlwaysAvailableDish);
        }
        
        private void addOptionsThatUserHasUnlocked() {
            dishUpgradesQuery
                .ToComponentDataArray<CDishUpgrade>(Unity.Collections.Allocator.Temp)
                .ToList<CDishUpgrade>()
                .Select(item => item.DishID).ToList()
                .ForEach(addDishOption);
        }

        private void addDishOption(int dishId) {
            if (Mod.loadedAvailableMenuOptions.Contains(dishId)) {
                return;
            }

            Dish dish = (Dish)GDOUtils.GetExistingGDO(dishId);

            if (dish == null) {
                dish = (Dish)GDOUtils.GetCustomGameDataObject(dishId).GameDataObject;
            }

            string dishName = dish != null ? dish.Name : dishId.ToString();

            Mod.loadedAvailableMenuOptions.Add(dishId);
            Mod.loadedAvailableMenuOptionNames.Add(dishName);
        }
    }
}
